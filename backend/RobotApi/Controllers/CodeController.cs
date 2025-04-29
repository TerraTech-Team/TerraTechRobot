using Microsoft.AspNetCore.Mvc;
using RobotApi.Models;

namespace RobotApi.Controllers;

[ApiController]
[Route("api/code")]
public class CodeController : ControllerBase
{
    private const string BinaryCodePath = "Resourses/robot_code.bin";

    [HttpPost("generate")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> GenerateCode([FromForm] CodeGenerateRequest request)
    {
        if (!IsRequestValid(request, out var errorResult))
            return errorResult;

        var binBytes = await System.IO.File.ReadAllBytesAsync(BinaryCodePath);
        try
        {

            return File(binBytes, "application/octet-stream", "robot_code.bin");
        }
        catch (Exception)
        {
            return InternalError("An error occurred while generating the robot code");
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

        if (request.Length == null || request.Width == null)
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
