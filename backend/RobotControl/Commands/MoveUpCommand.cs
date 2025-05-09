using RobotControl.Execution;

namespace RobotControl.Commands;

public class MoveUpCommand: IRobotCommand
{
    /* Перемещение по оси Y на +1 */
    public void Execute(IRobotContext context)
    {
        context.YAxis.Move(1);
    }
}