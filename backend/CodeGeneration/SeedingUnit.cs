namespace CodeGeneration
{
    public class SeedingUnit
    {
        public Servo GateServo { get; }
        public Servo WheelServo { get; }
        public StepperMotor MotorZ { get; }

        public bool IsUpState { get; set; } = true;
        private int currentContainerId = -1; //ещё не поворачивали

        public SeedingUnit(Servo gateServo, Servo wheelServo, StepperMotor motorZ)
        {
            GateServo = gateServo;
            WheelServo = wheelServo;
            MotorZ = motorZ;
        }

        private void SpinTo(int containerId)
        {
            if (containerId == currentContainerId)
                return;

            int angle = containerId * 36;
            WheelServo.RotateTo(angle);
            currentContainerId = containerId;
        }

        private void OpenGate() => GateServo.RotateTo(90);
        private void CloseGate() => GateServo.RotateTo(0);

        private void MoveUp(int steps = 60000)
        {
            MotorZ.Move(Direction.Forward, steps);
            IsUpState = true;
        }

        private void MoveDown(int steps = 60000)
        {
            MotorZ.Move(Direction.Backward, steps);
            IsUpState = false;
        }

        public void Seed(int containerId)
        {
            MoveDown();
            MoveUp();
            SpinTo(containerId);
            OpenGate();
            CloseGate();
        }
    }

}


