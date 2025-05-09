using SixLabors.ImageSharp.PixelFormats;

namespace RobotControl.Models;

public class SeedingParameters
{
    public float AreaWidthCm { get; set; }
    public float AreaHeightCm { get; set; }

    public int PixelsX { get; set; }
    public int PixelsY { get; set; }

    public Dictionary<Rgba32, (int ContainerId, string Name)> ColorToSeedContainerMap { get; set; } = new();

    public float PixelStepCm => AreaWidthCm / PixelsX;
}