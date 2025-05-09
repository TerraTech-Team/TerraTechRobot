using RobotControl.Execution;

namespace RobotControl.Commands;

public class MoveZCommand : IRobotCommand
{
    private readonly int steps;

    public MoveZCommand(int steps)
    {
        this.steps = steps;
    }

    public void Execute(IRobotContext context)
    {
        context.ZAxis.Move(steps);
    }
}