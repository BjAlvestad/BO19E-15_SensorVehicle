using System;
using Windows.Devices.Gpio;
using Windows.Foundation;
using VehicleEquipment;

namespace Communication.Vehicle
{
    public class PhysicalGpioPin : IGpioPin
    {
        private GpioPin _gpioPin;
        private bool _isInputPin;
        private Exception _exceptionWhenOpeningPin;

        /// <summary>
        /// Instantiates a GPIO pin which can be switched on or off.
        /// See <a href="https://docs.microsoft.com/en-us/windows/iot-core/learn-about-hardware/pinmappings/pinmappingsrpi#gpio-pin-overview"> see Raspberry Pi3 pin-mappings</a> for list of pins and their default pull-direction (0-8 up, 9-27 down).
        /// </summary>
        /// <param name="gpioNumber">This is GPIO number (not physical pin number). Default pull-direction for Raspberry Pi: 0-8 up, 9-27 down.</param>
        /// <param name="driveMode">If pin should be configured for input or output</param>
        /// <param name="setInitialValueHigh">Sets the initial state for the pin. Default is Low</param>
        public PhysicalGpioPin(int gpioNumber, GpioPinDriveMode driveMode, bool setInitialValueHigh = false)
        {
            try
            {
                PinNumber = gpioNumber;
                _gpioPin = GpioController.GetDefault().OpenPin(PinNumber);
                _gpioPin.Write(setInitialValueHigh ? GpioPinValue.High : GpioPinValue.Low);
                _gpioPin.SetDriveMode(driveMode);

                _isInputPin = driveMode == GpioPinDriveMode.Input || driveMode == GpioPinDriveMode.InputPullUp || driveMode == GpioPinDriveMode.InputPullDown;

                ErrorWhenOpeningPin = false;

                if (_isInputPin)
                {
                    _gpioPin.ValueChanged += ValueChangedOnInputPin;
                }
            }
            catch (Exception e)
            {
                ErrorWhenOpeningPin = true;
                _exceptionWhenOpeningPin = e;
            }
        }

        public event EventHandler PinValueInputChangedLow;
        public event EventHandler PinValueInputChangedHigh;

        public int PinNumber { get; }

        public bool ErrorWhenOpeningPin { get; }

        public bool PinHigh
        {
            get
            {
                ThrowExceptionIfErrorWhenOpeningPin();

                try
                {
                    return _gpioPin.Read() == GpioPinValue.High;
                }
                catch (Exception e)
                {
                    throw new Exception($"An error occured when trying to read from GPIO pin {PinNumber}", e);
                }
            }
            set
            {
                ThrowExceptionIfErrorWhenOpeningPin();
                ThrowExceptionIfDriveModeIsInput();
                // Calling the Write() method on an input pin is possible. However we decided to throw exception here when it is attempted for the following reason:
                // For our program; write calls to an input pin probably means that the programmer accidentally used the assign operator '=' when the intention was to use the equality comparison '=='

                try
                {
                    _gpioPin.Write(value ? GpioPinValue.High : GpioPinValue.Low);
                }
                catch (Exception e)
                {
                    throw new Exception($"An error occured when trying to switch output state of pin {PinNumber} to {(value ? "High" : "Low")}", e);
                }
            }
        }

        private void ThrowExceptionIfErrorWhenOpeningPin()
        {
            if (ErrorWhenOpeningPin)
            {
                throw new Exception($"Pin state can't be read or set, since GPIO pin {PinNumber} did not open successfully.\n" +
                                    $"Verify if this board has GPIO, and if {PinNumber} is a valid GPIO according to the SBC manual.\n" +
                                    $"If GPIO number is valid, try restarting the system.\n" +
                                    $"If GPIO is invalid/broken, reconnect to different GPIO, and change accordingly in App.xaml.cs", _exceptionWhenOpeningPin);
            }
        }

        private void ThrowExceptionIfDriveModeIsInput()
        {
            if (_isInputPin)
            {
                throw new InvalidOperationException($"Can't set state on pin {PinNumber} since it is configured as an input pin ({_gpioPin.GetDriveMode().ToString()})");
            }
        }


        private void ValueChangedOnInputPin(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            switch (args.Edge)
            {
                case GpioPinEdge.FallingEdge:
                    PinValueInputChangedLow?.Invoke(this, EventArgs.Empty);
                    break;
                case GpioPinEdge.RisingEdge:
                    PinValueInputChangedHigh?.Invoke(this, EventArgs.Empty);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}