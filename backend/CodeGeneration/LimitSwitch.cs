using System;
using System.Device.Gpio;


namespace CodeGeneration
{
    public class LimitSwitch
    {
        private readonly GpioController gpio;
        private readonly int pin;

        public string Name { get; }

        private DateTime lastTriggerTime;
        public bool IsTriggered
        {
            get
            {
                if (gpio.Read(pin) == PinValue.Low) // HIGH = выключен (EN не активен), LOW = включен (EN активен)
                {
                    if ((DateTime.UtcNow - lastTriggerTime).TotalMilliseconds > 50) // это защита от ложного срабатывания датчика (мы удостоверяемся, что касание не случайное)
                    {
                        lastTriggerTime = DateTime.UtcNow;
                        return true;
                    }
                }
                return false;
            }
        }

        public LimitSwitch(string name, int pin)
        {
            Name = name;
            this.pin = pin;
            gpio = new GpioController();
            gpio.OpenPin(this.pin, PinMode.InputPullUp);
        }

        public void Dispose()
        {
            gpio.ClosePin(pin);
            gpio.Dispose();
        }
    }
}
