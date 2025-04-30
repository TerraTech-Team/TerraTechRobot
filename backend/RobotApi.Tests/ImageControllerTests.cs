using System.Net;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.Testing;


namespace RobotApi.Tests;

public class ImageControllerTests
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
    public async Task ProcessImageReturnsPngWhenValidImageUploaded()
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "../../../TestAssets/sample.png");

        using var imageStream = File.OpenRead(path);
        using var content = new MultipartFormDataContent();

        content.Add(new StreamContent(imageStream)
        {
            Headers = { ContentType = new MediaTypeHeaderValue("image/png") }
        }, "image", "sample.png");

        content.Add(new StringContent("16"), "quality");

        var response = await client.PostAsync("/api/image/process", content);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(response.Content.Headers.ContentType?.MediaType, Is.EqualTo("image/png"));
    }


    [Test]
    public async Task ProcessImageReturnsBadRequestWhenImageIsMissing()
    {
        using var content = new MultipartFormDataContent();
        content.Add(new StringContent("16"), "quality");

        var response = await client.PostAsync("/api/image/process", content);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task ProcessImageReturnsUnsupportedMediaWhenFormatIsInvalid()
    {
        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("not an image"));
        using var content = new MultipartFormDataContent();

        content.Add(new StreamContent(stream)
        {
            Headers = { ContentType = new MediaTypeHeaderValue("text/plain") }
        }, "image", "invalid.txt");

        content.Add(new StringContent("32"), "quality");

        var response = await client.PostAsync("/api/image/process", content);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.UnsupportedMediaType));
    }

    [Test]
    public async Task ProcessImageReturnsUnprocessableWhenImageIsCorrupted()
    {
        var corruptedBytes = new byte[] { 0x00, 0x01, 0x02, 0x03 };
        using var stream = new MemoryStream(corruptedBytes);
        using var content = new MultipartFormDataContent();

        content.Add(new StreamContent(stream)
        {
            Headers = { ContentType = new MediaTypeHeaderValue("image/png") }
        }, "image", "corrupted.png");

        content.Add(new StringContent("32"), "quality");

        var response = await client.PostAsync("/api/image/process", content);

        Assert.That((int)response.StatusCode, Is.EqualTo(422));
    }

    [Test]
    public async Task ProcessImageReturnsBadRequestWhenQualityIsMissing()
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "../../../TestAssets/sample.png");

        using var imageStream = File.OpenRead(path);
        using var content = new MultipartFormDataContent();

        content.Add(new StreamContent(imageStream)
        {
            Headers = { ContentType = new MediaTypeHeaderValue("image/png") }
        }, "image", "sample.png");

        var response = await client.PostAsync("/api/image/process", content);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [TestCase("5")]
    [TestCase("500")]
    public async Task ProcessImageReturnsBadRequestWhenQualityIsOutOfRange(string quality)
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "../../../TestAssets/sample.png");

        using var imageStream = File.OpenRead(path);
        using var content = new MultipartFormDataContent();

        content.Add(new StreamContent(imageStream)
        {
            Headers = { ContentType = new MediaTypeHeaderValue("image/png") }
        }, "image", "sample.png");

        content.Add(new StringContent(quality), "quality");

        var response = await client.PostAsync("/api/image/process", content);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task ProcessImageReturnsHeadersWithUsedColors()
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "../../../TestAssets/sample.png");

        using var imageStream = File.OpenRead(path);
        using var content = new MultipartFormDataContent();

        content.Add(new StreamContent(imageStream)
        {
            Headers = { ContentType = new MediaTypeHeaderValue("image/png") }
        }, "image", "sample.png");

        content.Add(new StringContent("32"), "quality");

        var response = await client.PostAsync("/api/image/process", content);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        
        var colorHeaders = response.Headers
            .Where(h => h.Key.StartsWith("Color-", StringComparison.OrdinalIgnoreCase))
            .ToList();

        Assert.That(colorHeaders.Count, Is.GreaterThan(0), "Expected at least one Color-* header");
    }
    
    [Test]
    public async Task ProcessImageReturnsPngWhenJpegImageUploaded()
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "../../../TestAssets/sample.jpg");

        using var imageStream = File.OpenRead(path);
        using var content = new MultipartFormDataContent();

        content.Add(new StreamContent(imageStream)
        {
            Headers = { ContentType = new MediaTypeHeaderValue("image/jpeg") }
        }, "image", "sample.jpg");

        content.Add(new StringContent("32"), "quality");

        var response = await client.PostAsync("/api/image/process", content);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(response.Content.Headers.ContentType?.MediaType, Is.EqualTo("image/png"));
    }
}