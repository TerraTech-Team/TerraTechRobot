using RobotControl.Hardware;

namespace RobotControl.Core;

public class Robot : IRobot, ILimitSwitchObserver
{
    public Plane XYPlane { get; }
    public SeedingUnit Seeder { get; }

    public Robot(SeedingUnit seeder, Plane xyPlane)
    {
        Seeder = seeder;
        XYPlane = xyPlane;
    }

    public void Move(Direction direction, int steps)
    {
        switch (direction)
        {
            case Direction.Left:
                XYPlane.MotorX.Move(Direction.Backward, steps);
                XYPlane.X -= steps;
                break;
            case Direction.Right:
                XYPlane.MotorX.Move(Direction.Forward, steps);
                XYPlane.X += steps;
                break;
            case Direction.Up:
                XYPlane.MotorY.Move(Direction.Forward, steps);
                XYPlane.Y += steps;
                break;
            case Direction.Down:
                XYPlane.MotorY.Move(Direction.Backward, steps);
                XYPlane.Y -= steps;
                break;
        }
    }

    public void Home()
    {
        XYPlane.MotorX.Home(XYPlane.XMin);
        XYPlane.MotorY.Home(XYPlane.YMin);
        XYPlane.X = 0;
        XYPlane.Y = 0;
    }

    public void Seed(int containerId)
    {
        Seeder.Seed(containerId);
    }
    
    public void OnLimitSwitchTriggered(LimitSwitch limitSwitch)
    {
        Console.WriteLine($"[Robot] Limit switch triggered: {limitSwitch.Name}");
    }
}


