using System;
using Windows.Devices.Gpio;
using VehicleEquipment;

namespace Communication.Vehicle
{
    public class GpioOutputPin : IGpioOutputPin
    {
        private GpioPin _gpioPin;
        private Exception _exceptionWhenOpeningPin;

        /// <summary>
        /// Instantiates a GPIO pin which can be switched on or off.
        /// See <a href="https://docs.microsoft.com/en-us/windows/iot-core/learn-about-hardware/pinmappings/pinmappingsrpi#gpio-pin-overview"> see Raspberry Pi3 pin-mappings</a> for list of pins and their default pull-direction (0-8 up, 9-27 down).
        /// </summary>
        /// <param name="gpioNumber">This is GPIO number (not physical pin number). Default pull-direction for Raspberry Pi: 0-8 up, 9-27 down.</param>
        public GpioOutputPin(int gpioNumber)
        {
            try
            {
                PinNumber = gpioNumber;
                _gpioPin = GpioController.GetDefault().OpenPin(PinNumber);
                _gpioPin.Write(GpioPinValue.Low);
                _gpioPin.SetDriveMode(GpioPinDriveMode.Output);
                ErrorWhenOpeningPin = false;
            }
            catch (Exception e)
            {
                ErrorWhenOpeningPin = true;
                _exceptionWhenOpeningPin = e;
            }
        }

        private bool _setOutput;
        public bool SetOutput
        {
            get { return _setOutput; }
            set
            {
                if (ErrorWhenOpeningPin)
                {
                    throw new Exception($"Pin state can't be set, since GPIO pin {PinNumber} did not open successfully.\n" +
                                        $"Does this board have GPIO?\n" +
                                        $"Is the GPIO {PinNumber} actually a valid GPIO according to the SBC manual?.", _exceptionWhenOpeningPin);
                }

                try
                {
                    _gpioPin.Write(value ? GpioPinValue.High : GpioPinValue.Low);
                    _setOutput = value;
                }
                catch (Exception e)
                {
                    throw new Exception($"An error occured when trying to switch pin output state to {(value ? "High" : "Low")}", e);
                }
            }
        }

        public int PinNumber { get; }
        public bool ErrorWhenOpeningPin { get; }
    }
}
