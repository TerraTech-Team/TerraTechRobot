namespace RobotControl;

public interface IAxis
{
    public IStepperMotor Motor { get; }
    public IServoMotor Servo { get; }
}