using Microsoft.AspNetCore.Mvc;
using RobotApi.Models;
using RobotControl.Execution;
using RobotControl.Models;
using RobotControl.Planning;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace RobotApi.Controllers;

[ApiController]
[Route("api/debug")]
public class DebugController : ControllerBase
{
    [HttpPost("test-plan")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> TestPlan([FromForm] CodeGenerateRequest request)
    {
        if (!IsRequestValid(request, out var errorResult))
            return errorResult;

        try
        {
            Image<Rgba32> image;
            using (var stream = request.Image.OpenReadStream())
            {
                image = Image.Load<Rgba32>(stream);
            }

            var parameters = new SeedingParameters
            {
                AreaWidthCm = request.Width!.Value,
                AreaHeightCm = request.Length!.Value,
                PixelsX = image.Width,
                PixelsY = image.Height,
                ColorToSeedContainerMap = GetPaletteMap()
            };

            var planner = new BasicPlanner();
            var commands = planner.Plan(image, parameters);

            var context = new RobotContext(
                new MovementController("X"),
                new MovementController("Y"),
                new MovementController("Z"),
                new SeederController());

            Console.WriteLine("==== Выполнение команд ====");
            foreach (var cmd in commands)
            {
                cmd.Execute(context);
            }

            return Ok(new
            {
                Message = "План построен и команды выполнены (в консоли).",
                TotalCommands = commands.Count()
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Internal Server Error", message = ex.Message });
        }
    }

    private static bool IsRequestValid(CodeGenerateRequest request, out IActionResult? errorResult)
    {
        if (request.Image == null)
        {
            errorResult = new BadRequestObjectResult("Missing 'image' field in form data.");
            return false;
        }

        if (!request.Image.ContentType.ToLowerInvariant().StartsWith("image/png"))
        {
            errorResult = new ObjectResult("Only image/png is supported") { StatusCode = 415 };
            return false;
        }

        if (request.Length == null || request.Width == null)
        {
            errorResult = new BadRequestObjectResult("Missing 'length' or 'width'");
            return false;
        }

        if (request.Image.Length == 0)
        {
            errorResult = new UnprocessableEntityObjectResult("Image is empty");
            return false;
        }

        errorResult = null;
        return true;
    }

    private static Dictionary<Rgba32, (int, string)> GetPaletteMap() => new()
    {
        [new Rgba32(220, 20, 60)] = (0, "Crimson"),
        [new Rgba32(255, 140, 0)] = (1, "DarkOrange"),
        [new Rgba32(255, 215, 0)] = (2, "Gold"),
        [new Rgba32(60, 179, 113)] = (3, "MediumSeaGreen"),
        [new Rgba32(100, 149, 237)] = (4, "CornflowerBlue"),
        [new Rgba32(186, 85, 211)] = (5, "MediumOrchid"),
        [new Rgba32(245, 245, 245)] = (6, "WhiteSmoke"),
        [new Rgba32(30, 30, 30)] = (7, "DarkGray"),
        [new Rgba32(255, 182, 193)] = (8, "LightPink"),
        [new Rgba32(139, 69, 19)] = (9, "SaddleBrown")
    };
}

