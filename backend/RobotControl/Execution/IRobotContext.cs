namespace RobotControl.Execution;

public interface IRobotContext
{
    IMovementController XAxis { get; }
    IMovementController YAxis { get; }
    IMovementController ZAxis { get; }
    ISeederController Seeder { get; }
}
