namespace RobotControl.Execution;

public class MovementController: IMovementController
{
    private readonly string axis;
    public MovementController(string axis)
    {
        this.axis = axis;
    }

    public void Move(int steps) => Console.WriteLine($"[{axis}] Move {steps}");
    public void Home() => Console.WriteLine($"[{axis}] Home");
}