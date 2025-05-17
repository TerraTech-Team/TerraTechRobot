namespace RobotControl.Hardware;

public class Plane
{
    public StepperMotor MotorX { get; }
    public StepperMotor MotorY { get; }

    public LimitSwitch XMin { get; }
    public LimitSwitch XMax { get; }
    public LimitSwitch YMin { get; }
    public LimitSwitch YMax { get; }

    public int X { get; set; } = 0;
    public int Y { get; set; } = 0;

    public Plane(
        StepperMotor motorX,
        StepperMotor motorY,
        LimitSwitch xMin,
        LimitSwitch xMax,
        LimitSwitch yMin,
        LimitSwitch yMax)
    {
        MotorX = motorX;
        MotorY = motorY;
        XMin = xMin;
        XMax = xMax;
        YMin = yMin;
        YMax = yMax;
    }
}