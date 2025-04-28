using RobotApi.Models;
using ArtPixelisation;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace RobotApi.Controllers;

[ApiController]
[Route("api/image")]
public class ImageController : ControllerBase
{
    private static readonly List<Rgba32> defaultPalette =
    [
        new Rgba32(220, 20, 60),
        new Rgba32(255, 140, 0),
        new Rgba32(255, 215, 0),
        new Rgba32(60, 179, 113),
        new Rgba32(100, 149, 237),
        new Rgba32(186, 85, 211),
        new Rgba32(245, 245, 245),
        new Rgba32(30, 30, 30),
        new Rgba32(255, 182, 193),
        new Rgba32(139, 69, 19)
    ];
    
    [HttpPost("process")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> ProcessImage([FromForm] ImageProcessRequest request)
    {
        if (!IsRequestValid(request, out var errorResult))
            return errorResult;

        try
        {
            await using var imageStream = request.Image.OpenReadStream();
            using var image = await Image.LoadAsync<Rgba32>(imageStream);
            var result = PalettePixelation.Apply(defaultPalette, image, request.Quality);
            var outputStream = new MemoryStream();
            await result.Image.SaveAsPngAsync(outputStream);
            outputStream.Position = 0;
            
            AppendColorsToHeaders(result.UsedColors);

            return File(outputStream.ToArray(), "image/png", "processed.png");
        }
        catch (UnknownImageFormatException)
        {
            return Unprocessable("Invalid image file format.");
        }
        catch (ImageFormatException)
        {
            return Unprocessable("Image file is corrupted or unreadable.");
        }
        catch (Exception)
        {
            return StatusCode(500, new
            {
                error = "Internal Server Error",
                message = "Unexpected error while processing image"
            });
        }
    }
    
    private static bool IsRequestValid(ImageProcessRequest request, out IActionResult? errorResult)
    {
        if (request.Image == null)
        {
            errorResult = BadRequest("Missing 'image' field in form data.");
            return false;
        }

        var contentType = request.Image.ContentType.ToLowerInvariant();
        var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp", "image/tiff", "image/pjpeg" };
        if (!allowedTypes.Any(type => contentType.StartsWith(type)))
        {
            errorResult = UnsupportedMedia("image/jpeg, image/png, image/webp, image/tiff, image/pjpeg");
            return false;
        }

        errorResult = null;
        return true;
    }
    
    private void AppendColorsToHeaders(IEnumerable<Rgba32> colors)
    {
        var i = 1;
        foreach (var c in colors)
        {
            Response.Headers.Append($"Color-{i++}", $"{c.R},{c.G},{c.B}");
        }
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
}
