namespace RobotControl.Hardware;

using Core;

public class StepperMotor
{
    public string AxisName { get; }

    public StepperMotor(string axisName)
    {
        AxisName = axisName;
    }

    public void Move(Direction direction, int steps)
    {
        Console.WriteLine($"[{AxisName}] Move {direction} by {steps} steps");
    }

    public void Home(LimitSwitch endSwitch)
    {
        
        Console.WriteLine($"[{AxisName}] Homing until switch {endSwitch.Name} is triggered");
    }
}