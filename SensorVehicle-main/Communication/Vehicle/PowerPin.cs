using Windows.Devices.Gpio;

namespace Communication.Vehicle
{
    public class PowerPin
    {
        private readonly GpioPin _gpioPin;

        /// <summary>
        /// Instantiates a GPIO pin which can be switched on or off
        /// </summary>
        /// <param name="gpioNumber">This is GPIO number (not physical pin number)</param>
        public PowerPin(int gpioNumber)
        {
            _gpioPin = GpioController.GetDefault().OpenPin(gpioNumber);
            _gpioPin.Write(GpioPinValue.Low);
            _gpioPin.SetDriveMode(GpioPinDriveMode.Output);
        }

        private bool _power;
        public bool Power
        {
            get { return _power; }
            set
            {
                _power = value;
                _gpioPin.Write(value ? GpioPinValue.High : GpioPinValue.Low);
            }
        }
    }
}
