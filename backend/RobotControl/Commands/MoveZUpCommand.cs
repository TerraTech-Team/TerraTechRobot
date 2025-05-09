using RobotControl.Execution;

namespace RobotControl.Commands;

public class MoveZUpCommand : IRobotCommand
{
    public void Execute(IRobotContext context) => context.ZAxis.Move(1);
}