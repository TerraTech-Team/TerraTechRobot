namespace RobotControl.Execution;

public interface IMovementController
{
    void Move(int steps); // знаки + или - будут задавать направления
    void Home(); // на базу (0,0,0)
}