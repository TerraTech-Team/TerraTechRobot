namespace RobotControl;

public interface IRobot
{
    public IAxis XAxis { get; }
    public IAxis YAxis { get; }
    public IAxis ZAxis { get; }
    public IPlantingDevice PlantingDevice { get; }
}