using System.Net;
using System.Net.Http.Headers;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Testing;
using RobotApi.Models;

namespace RobotApi.Tests;

public class CodeControllerTests
{
    private WebApplicationFactory<Program> factory;
    private HttpClient client;

    [SetUp]
    public void SetUp()
    {
        factory = new WebApplicationFactory<Program>();
        client = factory.CreateClient();
    }

    [TearDown]
    public void TearDown()
    {
        client.Dispose();
        factory.Dispose();
    }

    [Test]
    public async Task GenerateCodeReturnsZipFileWhenValidRequest()
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "../../../TestAssets/sample.png");

        using var imageStream = File.OpenRead(path);
        using var content = new MultipartFormDataContent();

        content.Add(new StreamContent(imageStream)
        {
            Headers = { ContentType = new MediaTypeHeaderValue("image/png") }
        }, "image", "sample.png");

        content.Add(new StringContent("2000"), "length");
        content.Add(new StringContent("2000"), "width");

        var response = await client.PostAsync("/api/code/generate", content);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(response.Content.Headers.ContentType?.MediaType, Is.EqualTo("application/zip"));
        Assert.That(response.Content.Headers.ContentDisposition?.FileName, Is.EqualTo("CodeGeneration.zip"));
    }


    [Test]
    public async Task GenerateCodeReturnsBadRequestWhenImageIsMissing()
    {
        using var content = new MultipartFormDataContent();
        content.Add(new StringContent("100"), "length");
        content.Add(new StringContent("100"), "width");

        var response = await client.PostAsync("/api/code/generate", content);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task GenerateCodeReturnsUnsupportedMediaWhenImageTypeIsInvalid()
    {
        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("invalid"));
        using var content = new MultipartFormDataContent();

        content.Add(new StreamContent(stream)
        {
            Headers = { ContentType = new MediaTypeHeaderValue("text/plain") }
        }, "image", "text.txt");

        content.Add(new StringContent("100"), "length");
        content.Add(new StringContent("100"), "width");

        var response = await client.PostAsync("/api/code/generate", content);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.UnsupportedMediaType));
    }

    [Test]
    public async Task GenerateCodeReturnsUnprocessableWhenImageIsEmpty()
    {
        using var stream = new MemoryStream();
        using var content = new MultipartFormDataContent();

        content.Add(new StreamContent(stream)
        {
            Headers = { ContentType = new MediaTypeHeaderValue("image/png") }
        }, "image", "empty.png");

        content.Add(new StringContent("100"), "length");
        content.Add(new StringContent("100"), "width");

        var response = await client.PostAsync("/api/code/generate", content);

        Assert.That((int)response.StatusCode, Is.EqualTo(422));
    }

    [Test]
    public async Task GenerateCodeReturnsBadRequestWhenDimensionsAreMissing()
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "../../../TestAssets/sample.png");

        using var imageStream = File.OpenRead(path);
        using var content = new MultipartFormDataContent();

        content.Add(new StreamContent(imageStream)
        {
            Headers = { ContentType = new MediaTypeHeaderValue("image/png") }
        }, "image", "sample.png");

        var response = await client.PostAsync("/api/code/generate", content);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }
   

    private static bool ValidateModel(CodeGenerateRequest model, out List<ValidationResult> results)
    {
        var context = new ValidationContext(model);
        results = new List<ValidationResult>();
        return Validator.TryValidateObject(model, context, results, true);
    }

    [Test]
    public void ValidModelShouldPassValidation()
    {
        var model = new CodeGenerateRequest
        {
            Length = 100,
            Width = 100,
            Image = null
        };

        var isValid = ValidateModel(model, out var results);

        Assert.That(isValid, Is.True);
        Assert.That(results, Is.Empty);
    }

    [TestCase(0)]
    [TestCase(-10)]
    [TestCase(10001)]
    public void InvalidLengthShouldFailValidation(int invalidLength)
    {
        var model = new CodeGenerateRequest
        {
            Length = invalidLength,
            Width = 100
        };

        var isValid = ValidateModel(model, out var results);

        Assert.That(isValid, Is.False);
        Assert.That(results.Any(r => r.ErrorMessage!.Contains("Length must be between 1 and 10000.")), Is.True);
    }

    [TestCase(0)]
    [TestCase(-1)]
    [TestCase(20000)]
    public void InvalidWidthShouldFailValidation(int invalidWidth)
    {
        var model = new CodeGenerateRequest
        {
            Length = 100,
            Width = invalidWidth
        };

        var isValid = ValidateModel(model, out var results);

        Assert.That(isValid, Is.False);
        Assert.That(results.Any(r => r.ErrorMessage!.Contains("Width must be between 1 and 10000.")), Is.True);
    }

    [Test]
    public async Task GenerateCodeReturnsUnprocessableWhenStepIsTooSmall()
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "../../../TestAssets/sample.png");

        using var imageStream = File.OpenRead(path);
        using var content = new MultipartFormDataContent();

        content.Add(new StreamContent(imageStream)
        {
            Headers = { ContentType = new MediaTypeHeaderValue("image/png") }
        }, "image", "sample.png");

        content.Add(new StringContent("10"), "length");
        content.Add(new StringContent("10"), "width");

        var response = await client.PostAsync("/api/code/generate", content);

        Assert.That((int)response.StatusCode, Is.EqualTo(422));
        var body = await response.Content.ReadAsStringAsync();
        Assert.That(body, Does.Contain("Step size must be at least 1 cm"));
    }

}
