using RobotControl.Hardware;

namespace RobotControl.Core;

public class Robot : IRobot
{
    public Plane XYPlane { get; }
    public SeedingUnit Seeder { get; }

    public Robot(SeedingUnit seeder, Plane xyPlane)
    {
        Seeder = seeder;
        XYPlane = xyPlane;
    }
    
    public static Robot Create()
    {
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

        return new Robot(seedingUnit, plane);
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
        Console.WriteLine("Starting homing procedure...");
    
        // Хоминг по X
        Console.WriteLine("Homing X axis...");
        XYPlane.MotorX.Home(XYPlane.XMin);
        XYPlane.X = 0;
    
        // Хоминг по Y
        Console.WriteLine("Homing Y axis...");
        XYPlane.MotorY.Home(XYPlane.YMin);
        XYPlane.Y = 0;
    
        Console.WriteLine("Homing completed. Both axes at (0,0)");
    }

    public void Seed(int containerId)
    {
        Seeder.Seed(containerId);
    }
}


