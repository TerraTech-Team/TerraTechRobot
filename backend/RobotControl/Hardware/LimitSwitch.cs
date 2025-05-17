namespace RobotControl.Hardware;

public class LimitSwitch
{
    public string Name { get; }
    private bool Triggered;
    private readonly List<ILimitSwitchObserver> Observers = new();

    public LimitSwitch(string name)
    {
        Name = name;
    }

    public bool IsTriggered
    {
        get => Triggered;
        set
        {
            if (Triggered != value)
            {
                Triggered = value;

                if (Triggered)
                {
                    NotifyObservers();
                }
            }
        }
    }

    public void AddObserver(ILimitSwitchObserver observer)
    {
        if (!Observers.Contains(observer))
        {
            Observers.Add(observer);
        }
    }

    public void RemoveObserver(ILimitSwitchObserver observer)
    {
        Observers.Remove(observer);
    }

    private void NotifyObservers()
    {
        foreach (var observer in Observers)
        {
            observer.OnLimitSwitchTriggered(this);
        }
    }
}
