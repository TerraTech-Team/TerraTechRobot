namespace CodeGeneration
{
    public class Robot : IRobot
    {
        public Plane XYPlane { get; }
        public SeedingUnit Seeder { get; }

        public Robot(SeedingUnit seeder, Plane xyPlane)
        {
            Seeder = seeder;
            XYPlane = xyPlane;
        }

        public static Robot Create()
        {
            var xMin = new LimitSwitch("X_MIN", 32);
            var xMax = new LimitSwitch("X_MAX", 33);
            var yMin = new LimitSwitch("Y_MIN", 34);
            var yMax = new LimitSwitch("Y_MAX", 35);

            var motorX = new StepperMotor("X", 25, 27, 26); // STEP, DIR, EN
            var motorY = new StepperMotor("Y", 15, 13, 14); // STEP, DIR, EN

            var plane = new Plane(motorX, motorY, xMin, xMax, yMin, yMax);

            var motorZ = new StepperMotor("Z", 4, 16, 17); // STEP, DIR, EN
            var gateServo = new Servo("GateServo", 19); // PWM pin
            var wheelServo = new Servo("WheelServo", 18); // PWM pin

            var seedingUnit = new SeedingUnit(gateServo, wheelServo, motorZ);
            return new Robot(seedingUnit, plane);
        }

        public void Move(Direction direction, int steps)
        {
            switch (direction)
            {
                case Direction.Left:
                    XYPlane.MotorX.Move(Direction.Backward, steps);
                    break;
                case Direction.Right:
                    XYPlane.MotorX.Move(Direction.Forward, steps);
                    break;
                case Direction.Up:
                    XYPlane.MotorY.Move(Direction.Forward, steps);
                    break;
                case Direction.Down:
                    XYPlane.MotorY.Move(Direction.Backward, steps);
                    break;
            }
        }

        public void Home()
        {
            XYPlane.MotorX.Home(XYPlane.XMin);
            XYPlane.MotorY.Home(XYPlane.YMin);
        }

        public void CheckBoards()
        {
            XYPlane.MotorX.CheckBoard(XYPlane.XMax);
            XYPlane.MotorX.CheckBoard(XYPlane.YMax);
        }

        public void Seed(int containerId)
        {
            Seeder.Seed(containerId);
        }

        public void Dispose()
        {
            XYPlane.MotorX.Dispose();
            XYPlane.MotorY.Dispose();
            Seeder.MotorZ.Dispose();
            XYPlane.XMax.Dispose();
            XYPlane.YMax.Dispose();
            XYPlane.XMin.Dispose();
            XYPlane.YMin.Dispose();
            Seeder.WheelServo.Dispose();
            Seeder.GateServo.Dispose();
        }
    }
}
