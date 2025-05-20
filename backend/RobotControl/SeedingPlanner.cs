using RobotControl.Core;
using RobotControl.Hardware;
using RobotControl.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace RobotControl;

public class SeedingPlanner
{
    public List<Action> Plan(Robot robot, Image<Rgba32> image, SeedingParameters parameters)
    {
        var actions = new List<Action>();

        // Шаг 1: Хоминг по концевикам (границы)
        actions.Add(() => robot.Home());

        // Шаг 2: Определить границы — доехать до MAX
        //actions.Add(MoveUntilLimit(robot.XYPlane.MotorX, robot.XYPlane.XMax));
        //actions.Add(MoveUntilLimit(robot.XYPlane.MotorX, robot.XYPlane.YMax));

        // Шаг 3: Вернуться в (0,0)
        actions.Add(() => robot.Home());

        // Шаг 4: Засеивание по линиям — зигзаг
        for (var y = 0; y < parameters.PixelsY; y++)
        {
            var leftToRight = y % 2 == 0;

            for (var x = 0; x < parameters.PixelsX; x++)
            {
                var actualX = leftToRight ? x : parameters.PixelsX - 1 - x;
                var color = image[actualX, y];

                if (!parameters.ColorToContainerMap.TryGetValue(color, out int containerId))
                    continue;

                if (x > 0 || y > 0)
                {
                    if (x > 0)
                        actions.Add(() => robot.Move(leftToRight ? Direction.Right : Direction.Left, (int)parameters.StepX));

                    if (y > 0 && x == 0)
                        actions.Add(() => robot.Move(Direction.Down, (int)parameters.StepY));
                }

                actions.Add(() => robot.Seed(containerId));
            }
        }
        
        actions.Add(() => robot.Home());

        return actions;
    }
    
    private Action MoveUntilLimit(StepperMotor motor, LimitSwitch limit)
    {
        return () =>
        {
            while (!limit.IsTriggered)
            {
                motor.Move(Direction.Forward, 1);
            }
        };
    }
}
