using Microsoft.AspNetCore.Mvc;
using RobotApi.Models;
using RobotControl;
using RobotControl.Models;
using RobotControl.Core;
using RobotControl.Hardware;
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
                ColorToContainerMap = GetPaletteMap()
            };

            // Создаём виртуальные концевики
            var xMin = new LimitSwitch("X_MIN");
            var xMax = new LimitSwitch("X_MAX");
            var yMin = new LimitSwitch("Y_MIN");
            var yMax = new LimitSwitch("Y_MAX");

            // Создаём моторы
            var motorX = new StepperMotor("X");
            var motorY = new StepperMotor("Y");

            // Плоскость
            var plane = new Plane(motorX, motorY, xMin, xMax, yMin, yMax);

            // Z-мотор и серво-приводы
            var motorZ = new StepperMotor("Z");
            var gateServo = new Servo("GateServo");
            var wheelServo = new Servo("WheelServo");

            // Посевной модуль
            var seedingUnit = new SeedingUnit(gateServo, wheelServo, motorZ);

            // Робот
            var robot = new Robot(seedingUnit, plane);

            // Планировщик
            var planner = new SeedingPlanner();
            var actions = planner.Plan(robot, image, parameters);

            Console.WriteLine("==== Выполнение команд ====");

            foreach (var action in actions)
            {
                action.Invoke(); // Выполняем действия (движение/посев)
            }

            return Ok(new
            {
                Message = "План построен и действия выполнены (в консоли).",
                TotalActions = actions.Count
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

    private static Dictionary<Rgba32, int> GetPaletteMap() => new()
    {
        [new Rgba32(220, 20, 60)] = 0,
        [new Rgba32(255, 140, 0)] = 1,
        [new Rgba32(255, 215, 0)] = 2,
        [new Rgba32(60, 179, 113)] = 3,
        [new Rgba32(100, 149, 237)] = 4,
        [new Rgba32(186, 85, 211)] = 5,
        [new Rgba32(245, 245, 245)] = 6,
        [new Rgba32(30, 30, 30)] = 7,
        [new Rgba32(255, 182, 193)] = 8,
        [new Rgba32(139, 69, 19)] = 9
    };
}


