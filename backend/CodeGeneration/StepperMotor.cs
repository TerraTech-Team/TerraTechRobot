using System;
using System.Device.Gpio;
using System.Threading;

namespace CodeGeneration
{
    public class StepperMotor
    {
        private readonly GpioController gpio;
        private readonly int stepPin;
        private readonly int dirPin;
        private readonly int enablePin;

        public string AxisName { get; }

        public StepperMotor(string axisName, int stepPin, int dirPin, int enablePin)
        {
            AxisName = axisName;
            this.stepPin = stepPin;
            this.dirPin = dirPin;
            this.enablePin = enablePin;

            gpio = new GpioController();
            gpio.OpenPin(this.stepPin, PinMode.Output); //открытие пинов
            gpio.OpenPin(this.dirPin, PinMode.Output);
            gpio.OpenPin(this.enablePin, PinMode.Output);
            // HIGH = выключен (EN не активен), LOW = включен (EN активен)
            gpio.Write(this.enablePin, PinValue.High); //драйвер изначально ВЫКЛЮЧЕН
        }

        private void EnableDriver()
        {
            gpio.Write(enablePin, PinValue.Low); // ВКЛЮЧИТЬ драйвер (решил не включать в конструкторе и делать так, чтобы меньше энергии потребляло и не нагревалось)
        }



        public void Move(Direction direction, int steps)
        {
            EnableDriver();
            gpio.Write(dirPin, direction == Direction.Forward ? PinValue.High : PinValue.Low);

            for (int i = 0; i < steps; i++)
            {
                gpio.Write(stepPin, PinValue.High);
                Thread.Sleep(1); // задержка для корректной работы шаговика, иначе он будет пропускать шаги, пауза обязательна
                gpio.Write(stepPin, PinValue.Low);
                Thread.Sleep(1); // задержка для корректной работы шаговика, иначе он будет пропускать шаги, пауза обязательна
            }
            DisableDriver();
        }

        public void Home(LimitSwitch endSwitch)
        {
            while (!endSwitch.IsTriggered)
            {
                Move(Direction.Backward, 1);
                Thread.Sleep(2); // задержка чтобы не перегружать проц постоянными обращениями к концевику
            }
        }

        public void CheckBoard(LimitSwitch endSwitch)
        {
            while (!endSwitch.IsTriggered)
            {
                Move(Direction.Forward, 1);
                Thread.Sleep(2); // задержка чтобы не перегружать проц постоянными обращениями к концевику
            }
        }

        public void DisableDriver()
        {
            gpio.Write(enablePin, PinValue.High); // Отключить драйвер
        }

        public void Dispose() //окончание работы движка
        {
            gpio.ClosePin(stepPin);
            gpio.ClosePin(dirPin);
            gpio.ClosePin(enablePin);
            gpio.Dispose();
        }

    }
}
