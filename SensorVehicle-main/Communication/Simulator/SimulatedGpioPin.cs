using System;
using Windows.Devices.Gpio;
using Windows.System.Threading;
using VehicleEquipment;

namespace Communication.Simulator
{
    public class SimulatedGpioPin : IGpioPin
    {
        private Exception _exceptionWhenOpeningPin;
        private GpioPinDriveMode _pinMode;
        private bool _isInputPin;
        private ThreadPoolTimer _interruptTimer;

        public SimulatedGpioPin(int gpioNumber, GpioPinDriveMode driveMode, bool setInitialValueHigh = false, SimulatedVehicleCommunication communicaton = null, int interruptInterval = 0)
        {
            try
            {
                PinNumber = gpioNumber;
                _pinMode = driveMode;

                if (gpioNumber < 0)
                {
                    throw new Exception("Pin number is out of range");
                }

                _isInputPin = driveMode == GpioPinDriveMode.Input || driveMode == GpioPinDriveMode.InputPullUp || driveMode == GpioPinDriveMode.InputPullDown;

                if (_isInputPin)
                {
                    if(communicaton != null) communicaton.NewDataReceived += OnNewDataReceivedFromSensor;

                    _interruptTimer = StartInterruptTimer(interruptInterval);
                    _pinHigh = true;
                }
                else
                {
                    PinHigh = setInitialValueHigh;
                }

                ErrorWhenOpeningPin = false;
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

        private bool _pinHigh;
        public bool PinHigh
        {
            get
            {
                ThrowExceptionIfErrorWhenOpeningPin();
                return _pinHigh;
            }
            set
            {
                ThrowExceptionIfErrorWhenOpeningPin();
                ThrowExceptionIfDriveModeIsInput();

                _pinHigh = value;
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
            if (_pinMode == GpioPinDriveMode.Input || _pinMode == GpioPinDriveMode.InputPullUp || _pinMode == GpioPinDriveMode.InputPullDown)
            {
                throw new InvalidOperationException($"Can't set state on pin {PinNumber} since it is configured as an input pin ({_pinMode.ToString()})");
            }
        }

        private ThreadPoolTimer StartInterruptTimer(int millisecondsBetweenInterrupts)
        {
            if (millisecondsBetweenInterrupts <= 0) return null;

            return ThreadPoolTimer.CreatePeriodicTimer(
                (ThreadPoolTimer timer) =>
                {
                    if (_pinHigh == false)
                    {
                        _pinHigh = true;
                        PinValueInputChangedHigh?.Invoke(this, EventArgs.Empty);
                    }
                }, 
                TimeSpan.FromMilliseconds(millisecondsBetweenInterrupts));
        }

        private void OnNewDataReceivedFromSensor(object sender, EventArgs args)
        {
            _pinHigh = false;
            PinValueInputChangedLow?.Invoke(this, EventArgs.Empty);
        }
    }
}
