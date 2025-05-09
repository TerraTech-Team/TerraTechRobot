namespace RobotControl.Execution;

/* активация модуля по засеиванию (входные параметры доработаю, когда будет полностью понятен алгоритм засеивания*/
public interface ISeederController
{
    void Activate(int milliseconds, int containerId, string containerName);
}