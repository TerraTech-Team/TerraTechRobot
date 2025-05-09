using RobotControl.Execution;

namespace RobotControl.Commands;

public class MoveDownCommand:IRobotCommand
{
    /* перемещение по оси Y на -1 */
    public void Execute(IRobotContext context)
    {
        context.YAxis.Move(-1);
    }
}