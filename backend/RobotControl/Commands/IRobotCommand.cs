using RobotControl.Execution;

namespace RobotControl.Commands;

public interface IRobotCommand
{
    void Execute(IRobotContext context);
}