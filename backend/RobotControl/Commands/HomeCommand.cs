using RobotControl.Execution;

namespace RobotControl.Commands;

public class HomeCommand: IRobotCommand
{
    /* Возвращение на базу (0,0,0) */
    public void Execute(IRobotContext context)
    {
        context.XAxis.Home();
        context.YAxis.Home();
        context.ZAxis.Home();
    }
}