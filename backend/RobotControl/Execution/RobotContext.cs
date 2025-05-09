namespace RobotControl.Execution;

public class RobotContext(
    IMovementController xAxis,
    IMovementController yAxis,
    IMovementController zAxis,
    ISeederController seeder)
    : IRobotContext
{
    public IMovementController XAxis { get; } = xAxis;
    public IMovementController YAxis { get; } = yAxis;
    public IMovementController ZAxis { get; } = zAxis;
    public ISeederController Seeder { get; } = seeder;

    public void HomeAll()
    {
        XAxis.Home();
        YAxis.Home();
        ZAxis.Home();
    }
}
