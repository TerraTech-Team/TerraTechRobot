using RobotControl.Execution;

namespace RobotControl.Commands;

public class MoveLeftCommand: IRobotCommand
{
    /* перемещение по оси X на -1 */
    public void Execute(IRobotContext context)
    {
        context.XAxis.Move(-1);
    }
}