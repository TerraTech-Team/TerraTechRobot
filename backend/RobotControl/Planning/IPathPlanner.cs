using RobotControl.Commands;
using RobotControl.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace RobotControl.Planning;

public interface IPathPlanner
{
    IEnumerable<IRobotCommand> Plan(Image<Rgba32> image, SeedingParameters parameters);
}