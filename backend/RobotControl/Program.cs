using System.Device.Gpio;
using System.Device.Pwm;

namespace FlowerSeedingRobot;

public class Program
{
    // X ось
    private const int X_STEP = 21;
    private const int X_DIR = 22;
    private const int X_EN = 23;
    private const int X_MIN = 32;
    private const int X_MAX = 33;

    // Y ось
    private const int Y_STEP = 18;
    private const int Y_DIR = 19;
    private const int Y_EN = 5;
    private const int Y_MIN = 34;
    private const int Y_MAX = 35;

    // Z ось
    private const int Z_STEP = 16;
    private const int Z_DIR = 17;
    private const int Z_EN = 4;

    // Сервоприводы
    private const int GateServoPin = 25;
    private const int WheelServoPin = 26;

    // Контроллеры
    private static GpioController gpio;
    private static PwmChannel gateServo;
    private static PwmChannel wheelServo;

    public static void Main()
    {
        // Настройка GPIO
        gpio = new GpioController();

        SetupMotorPins(X_STEP, X_DIR, X_EN);
        SetupMotorPins(Y_STEP, Y_DIR, Y_EN);
        SetupMotorPins(Z_STEP, Z_DIR, Z_EN);

        SetupLimitSwitch(X_MIN);
        SetupLimitSwitch(X_MAX);
        SetupLimitSwitch(Y_MIN);
        SetupLimitSwitch(Y_MAX);

        // Настройка PWM для сервоприводов
        gateServo = PwmChannel.CreateFromPin(GateServoPin, 50);
        gateServo.Start();
        wheelServo = PwmChannel.CreateFromPin(WheelServoPin, 50);
        wheelServo.Start();

        // Пример: Хоминг по оси X до MIN
        HomeAxis(X_STEP, X_DIR, X_MIN, directionIsForward: false);

        // Пример: Движение по X
        MoveStepper(X_STEP, X_DIR, steps: 200, forward: true, delayUs: 1000);

        // Пример: Управление сервоприводом
        SetServoAngle(gateServo, 90); // открыть заслонку
        Thread.Sleep(500);
        SetServoAngle(gateServo, 0);  // закрыть заслонку
    }

    private static void SetupMotorPins(int step, int dir, int en)
    {
        gpio.OpenPin(step, PinMode.Output);
        gpio.OpenPin(dir, PinMode.Output);
        gpio.OpenPin(en, PinMode.Output);
        gpio.Write(en, PinValue.Low); // Включить драйвер
    }

    private static void SetupLimitSwitch(int pin)
    {
        gpio.OpenPin(pin, PinMode.InputPullUp); // Подтяжка к питанию
    }

    private static void MoveStepper(int stepPin, int dirPin, int steps, bool forward, int delayUs)
    {
        gpio.Write(dirPin, forward ? PinValue.High : PinValue.Low);

        for (int i = 0; i < steps; i++)
        {
            gpio.Write(stepPin, PinValue.High);
            DelayMicroseconds(delayUs);
            gpio.Write(stepPin, PinValue.Low);
            DelayMicroseconds(delayUs);
        }
    }

    private static void HomeAxis(int stepPin, int dirPin, int limitSwitchPin, bool directionIsForward)
    {
        gpio.Write(dirPin, directionIsForward ? PinValue.High : PinValue.Low);

        // Двигаемся, пока концевик не сработает
        while (gpio.Read(limitSwitchPin) == PinValue.High)
        {
            gpio.Write(stepPin, PinValue.High);
            DelayMicroseconds(1000);
            gpio.Write(stepPin, PinValue.Low);
            DelayMicroseconds(1000);
        }

        Console.WriteLine($"Ось с шагом на пине {stepPin} достигла концевика {limitSwitchPin}");
    }

    private static void SetServoAngle(PwmChannel pwm, int angle)
    {
        // Расчет ширины импульса (0.5мс–2.5мс для SG90, 50Гц = 20мс период)
        double pulseWidthMs = 0.5 + (angle / 180.0) * 2.0;
        double dutyCycle = pulseWidthMs / 20.0;
        pwm.DutyCycle = dutyCycle;
    }

    private static void DelayMicroseconds(int us)
    {
        long ticks = us * (System.Diagnostics.Stopwatch.Frequency / 1_000_000);
        var sw = System.Diagnostics.Stopwatch.StartNew();
        while (sw.ElapsedTicks < ticks) { }
    }
}