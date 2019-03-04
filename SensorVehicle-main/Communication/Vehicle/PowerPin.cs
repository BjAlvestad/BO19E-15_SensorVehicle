using System;
using Windows.Devices.Gpio;

namespace Communication.Vehicle
{
    public class PowerPin
    {
        private readonly GpioPin _gpioPin;

        /// <summary>
        /// Instantiates a GPIO pin which can be switched on or off.
        /// See <a href="https://docs.microsoft.com/en-us/windows/iot-core/learn-about-hardware/pinmappings/pinmappingsrpi#gpio-pin-overview"> see Raspberry Pi3 pin-mappings</a> for list of pins and their default pull-direction (0-8 up, 9-27 down).
        /// </summary>
        /// <param name="gpioNumber">This is GPIO number (not physical pin number)</param>
        public PowerPin(int gpioNumber)
        {
            try
            {
                _gpioPin = GpioController.GetDefault().OpenPin(gpioNumber);
                _gpioPin.Write(GpioPinValue.Low);
                _gpioPin.SetDriveMode(GpioPinDriveMode.Output);
                PinFailedToOpen = false;
            }
            catch (Exception e)
            {
                PinFailedToOpen = true;
                throw new Exception($"An error when trying to open GPIO-pin {gpioNumber}:\n{e.Message}\n\nDetails: \n{e}\n****************\n\n)");
            }
        }

        private bool _power;
        public bool Power
        {
            get { return _power; }
            set
            {
                try
                {
                    _gpioPin.Write(value ? GpioPinValue.High : GpioPinValue.Low);
                    _power = value;
                }
                catch (Exception e)
                {
                    throw new Exception($"An error occured when trying to switch power state to {(value ? "ON" : "OFF")}:\n{e.Message}\n\nDetails: \n{e}\n****************\n\n)");
                }
            }
        }

        public bool PinFailedToOpen { get; private set; }
    }
}
