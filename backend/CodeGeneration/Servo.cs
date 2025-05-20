using System.Device.Pwm;

namespace CodeGeneration
{
    public class Servo
    {
        // !!!текщая реализация требует корректировки входных параметров империческим путём
        private readonly PwmChannel pwm;
        private readonly int minPulseUs;
        private readonly int maxPulseUs;
        private readonly int maxAngle;
        private double currentAngle;

        public string Name { get; }

        public Servo(string name, int pwmPin, int frequency = 50, int minPulseUs = 1000, int maxPulseUs = 2000, int maxAngle = 360)
        {
            Name = name;
            pwm = PwmChannel.CreateFromPin(pwmPin, frequency);
            pwm.Start();
            this.minPulseUs = minPulseUs;
            this.maxPulseUs = maxPulseUs;
            this.maxAngle = maxAngle;
            Stop(); // Остановить при старте
        }

        public void RotateTo(double angle)
        {
            if (angle < 0) angle = 0;
            if (angle > 360) angle = 360;
            currentAngle = angle;

            // импульс от minPulseUs до maxPulseUs
            double pulseWidthUs = minPulseUs + ((angle / maxAngle) * (maxPulseUs - minPulseUs));
            double dutyCycle = (pulseWidthUs / 1000.0) / 20.0; // 50 Гц = 20 мс период

            pwm.DutyCycle = dutyCycle;
        }

        public void Stop()
        {
            pwm.DutyCycle = (1.5 / 20.0); // Позиция покоя (примерно середина)
        }

        public void Dispose()
        {
            pwm.Dispose();
        }

        public double GetCurrentAngle() => currentAngle;
    }
}

