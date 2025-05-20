namespace RobotControl.Hardware;

public class LimitSwitch
{
    public string Name { get; }

    private bool triggered;
    public bool IsTriggered
    {
        get => triggered;
        set
        {
            if (triggered != value)
            {
                triggered = value;
                Console.WriteLine($"[{Name}] Switch {(value ? "TRIGGERED" : "RELEASED")}");
            }
        }
    }

    public LimitSwitch(string name)
    {
        Name = name;
    }
}