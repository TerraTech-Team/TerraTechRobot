namespace RobotControl.Hardware;

public class Servo
{
    public string Name { get; }

    public Servo(string name)
    {
        Name = name;
    }

    public void RotateTo(int position)
    {
        Console.WriteLine($"[{Name}] Rotate to position {position}");
    }
}