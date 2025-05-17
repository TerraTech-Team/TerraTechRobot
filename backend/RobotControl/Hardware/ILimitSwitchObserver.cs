namespace RobotControl.Hardware;

public interface ILimitSwitchObserver
{
    void OnLimitSwitchTriggered(LimitSwitch limitSwitch);
}