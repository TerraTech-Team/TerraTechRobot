using RobotControl.Commands;
using RobotControl.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace RobotControl.Planning;

public class BasicPlanner : IPathPlanner
{
    public IEnumerable<IRobotCommand> Plan(Image<Rgba32> image, SeedingParameters parameters)
    {
        var commands = new List<IRobotCommand>();
        var currentPosition = new Position { X = 0, Y = 0, Z = 0 };
        bool leftToRight = true;

        for (int y = 0; y < image.Height; y++)
        {
            var xRange = leftToRight
                ? Enumerable.Range(0, image.Width)
                : Enumerable.Range(0, image.Width).Reverse();

            foreach (int x in xRange)
            {
                var pixel = image[x, y];

                if (parameters.ColorToSeedContainerMap.TryGetValue(pixel, out var containerInfo))
                {
                    var (containerId, colorName) = containerInfo;

                    // Move to position
                    commands.AddRange(MoveTo(currentPosition, x, y));
                    currentPosition.X = x;
                    currentPosition.Y = y;

                    // Засеивание
                    commands.Add(new MoveZCommand(-1)); // опустить Z
                    commands.Add(new SeedCommand(containerId, colorName)); // активировать контейнер
                    commands.Add(new MoveZCommand(1));  // поднять Z
                }
            }

            leftToRight = !leftToRight;
        }

        commands.Add(new HomeCommand());
        return commands;
    }

    private IEnumerable<IRobotCommand> MoveTo(Position current, int targetX, int targetY)
    {
        var cmds = new List<IRobotCommand>();

        int deltaX = targetX - current.X;
        int deltaY = targetY - current.Y;

        if (deltaX > 0)
            cmds.AddRange(Enumerable.Repeat<IRobotCommand>(new MoveRightCommand(), deltaX));
        else if (deltaX < 0)
            cmds.AddRange(Enumerable.Repeat<IRobotCommand>(new MoveLeftCommand(), -deltaX));

        if (deltaY > 0)
            cmds.AddRange(Enumerable.Repeat<IRobotCommand>(new MoveUpCommand(), deltaY));
        else if (deltaY < 0)
            cmds.AddRange(Enumerable.Repeat<IRobotCommand>(new MoveDownCommand(), -deltaY));

        return cmds;
    }
}


