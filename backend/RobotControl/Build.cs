namespace RobotControl;

public class Build
{
    public static void Main(IRobot robot)
    {
        robot.XAxis.Motor.Move(10);
        robot.YAxis.Motor.Move(10);
        
        robot.PlantingDevice.Plant(Seeds.Blue);
        
        robot.XAxis.Motor.Move(10);
        robot.YAxis.Motor.Move(10);
        
        robot.PlantingDevice.Plant(Seeds.Green);
        
        robot.XAxis.Motor.Move(10);
        robot.YAxis.Motor.Move(10);
        
        robot.PlantingDevice.Plant(Seeds.Red);
        
        robot.XAxis.Motor.Move(10);
        robot.YAxis.Motor.Move(10);
    }
}