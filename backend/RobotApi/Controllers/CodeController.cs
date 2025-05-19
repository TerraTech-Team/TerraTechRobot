using Microsoft.AspNetCore.Mvc;
using RobotApi.Models;
using RobotApi.Services;
using System.IO.Compression;

namespace RobotApi.Controllers;

[ApiController]
[Route("api/code")]
public class CodeController : ControllerBase
{
    private readonly SeedingPreparationService prepService;
    private readonly NanoCodeInjectionService injectionService;
    private readonly string backendRootPath;

    public CodeController(IWebHostEnvironment env)
    {
        prepService = new SeedingPreparationService();
        injectionService = new NanoCodeInjectionService();
        backendRootPath = Path.Combine(env.ContentRootPath, "..");
    }

    [HttpPost("generate")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> GenerateCode([FromForm] CodeGenerateRequest request)
    {
        if (!IsRequestValid(request, out var errorResult))
            return errorResult;

        try
        {
            await using var originalStream = request.Image!.OpenReadStream();
            using var memoryStream = new MemoryStream();
            await originalStream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            var parameters = new SeedingParameters
            {
                AreaWidthCm = request.Width!.Value,
                AreaHeightCm = request.Length!.Value
            };
            var (preparedParams, colorMap) = await prepService.PrepareAsync(memoryStream, parameters);

            var generationRoot = Path.GetFullPath(Path.Combine(backendRootPath, "CodeGeneration"));
            injectionService.GenerateFirmwareDataFile(colorMap, preparedParams, generationRoot);

            var zipFilePath = Path.Combine(Path.GetTempPath(), $"CodeGeneration_{Guid.NewGuid()}.zip");
            if (System.IO.File.Exists(zipFilePath))
                System.IO.File.Delete(zipFilePath);

            ZipFile.CreateFromDirectory(generationRoot, zipFilePath);

            var zipBytes = await System.IO.File.ReadAllBytesAsync(zipFilePath);
            return File(zipBytes, "application/zip", "CodeGeneration.zip");
        }
        catch (Exception ex)
        {
            return InternalError("An error occurred while generating the project zip: " + ex.Message);
        }
    }

    private static bool IsRequestValid(CodeGenerateRequest request, out IActionResult? errorResult)
    {
        if (request.Image == null)
        {
            errorResult = BadRequest("Missing 'image' field in form data.");
            return false;
        }

        var contentType = request.Image.ContentType.ToLowerInvariant();
        if (!contentType.StartsWith("image/png"))
        {
            errorResult = UnsupportedMedia("Only image/png is supported");
            return false;
        }

        if (request.Length == null || request.Width == null || request.Length <= 0 || request.Width <= 0)
        {
            errorResult = BadRequest("Invalid or missing 'length' or 'width' parameter");
            return false;
        }

        if (request.Image.Length == 0)
        {
            errorResult = Unprocessable("Cannot generate code from the provided image");
            return false;
        }

        errorResult = null;
        return true;
    }

    private static IActionResult BadRequest(string message) => new BadRequestObjectResult(new
    {
        error = "Bad Request",
        message
    });

    private static IActionResult UnsupportedMedia(string message) => new ObjectResult(new
    {
        error = "Unsupported Media Type",
        message
    })
    {
        StatusCode = 415
    };

    private static IActionResult Unprocessable(string message) => new UnprocessableEntityObjectResult(new
    {
        error = "Unprocessable Entity",
        message
    });

    private static IActionResult InternalError(string message) => new ObjectResult(new
    {
        error = "Internal Server Error",
        message
    })
    {
        StatusCode = 500
    };
}

