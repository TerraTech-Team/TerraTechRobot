using RobotControl.Execution;

namespace RobotControl.Commands;

public class MoveZDownCommand : IRobotCommand
{
    public void Execute(IRobotContext context) => context.ZAxis.Move(-1);
}