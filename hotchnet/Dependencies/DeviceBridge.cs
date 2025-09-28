using System.IO.Ports;
using System.Device.Gpio;

namespace hotchnet.Dependencies;

public class DeviceBridge : IDisposable
{
    private const int ControlLED = 17;
    private const int ControlMotor = 27;
    private GpioController Controller;
    private SerialPort SerialPort;

    public bool LED
    {
        get
        {
            return Controller.Read(ControlLED) == PinValue.High;
        }
        set
        {
            Controller.Write(ControlLED, value == true ? PinValue.High : PinValue.Low);
        }
    }

    public bool Motor
    {
        get
        {
            return Controller.Read(ControlMotor) == PinValue.High;
        }
        set
        {
            Controller.Write(ControlMotor, value == true ? PinValue.High : PinValue.Low);
        }
    }

    public DeviceBridge()
    {
        Controller = new();
        Controller.OpenPin(ControlLED, PinMode.Output, PinValue.Low);
        Controller.OpenPin(ControlMotor, PinMode.Output, PinValue.Low);

        SerialPort = new("/dev/serial0", 115200)
        {
            ReadTimeout = 500,
            WriteTimeout = 500
        };
        SerialPort.Open();
    }

    public void Dispose()
    {
        SerialPort.Close();
        SerialPort.Dispose();

        Controller.ClosePin(ControlMotor);
        Controller.ClosePin(ControlLED);
        Controller.Dispose();

        GC.SuppressFinalize(this);
    }
}
