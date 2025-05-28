namespace CodeGeneration
{
    public interface IRobot
    {
        void Move(Direction direction, int steps);
        void Home();
        void Seed(int containerId);
    }
}
