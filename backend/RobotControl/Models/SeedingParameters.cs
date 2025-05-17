using SixLabors.ImageSharp.PixelFormats;

namespace RobotControl.Models;

public class SeedingParameters
{
    public float AreaWidthCm { get; set; }
    public float AreaHeightCm { get; set; }

    public int PixelsX { get; set; }
    public int PixelsY { get; set; }

    public Dictionary<Rgba32, int> ColorToContainerMap { get; set; } = new();

    public float StepX => AreaWidthCm / PixelsX;
    public float StepY => AreaHeightCm / PixelsY;
}