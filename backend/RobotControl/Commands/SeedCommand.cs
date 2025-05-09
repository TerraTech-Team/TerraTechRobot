using RobotControl.Execution;

namespace RobotControl.Commands;

public class SeedCommand : IRobotCommand
{
    private readonly int durationMs;
    private readonly int containerId;
    private readonly string containerName;

    public SeedCommand(int containerId, string containerName, int durationMs = 500)
    {
        this.durationMs = durationMs;
        this.containerId = containerId;
        this.containerName = containerName;
    }

    public void Execute(IRobotContext context)
    {
        context.Seeder.Activate(durationMs, containerId, containerName);
    }
}