namespace RobotControl.Execution;

public class SeederController: ISeederController
{
    public void Activate(int milliseconds, int containerId, string containerName)
    {
        Console.WriteLine($"[Seeder] Using container {containerId} ({containerName})");
        Console.WriteLine($"[Seeder] Activate container {containerId} for {milliseconds} ms");

        // Тут будет логика аппаратного вызова, например: hardware.Seed(containerId, milliseconds);
    }
}