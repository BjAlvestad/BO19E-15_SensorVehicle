using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Helpers;

namespace Communication.Vehicle
{
    public class Power : ThreadSafeNotifyPropertyChanged, IPower
    {
        //TODO: Extract pin out to separate class
        private readonly PowerPin _lidarPin;
        private readonly PowerPin _ultrasoundPin;
        private readonly PowerPin _wheelsPin;
        private readonly PowerPin _encoderPin;
        private readonly PowerPin _spare1Pin;
        private readonly PowerPin _spare2Pin;
        private readonly PowerPin _spare3Pin;

        public Power()
        {
            // For Raspberry Pi 3b, the GPIO 0-8 have pull high as default (on), while GPIO 9-27 have pull low (off)
            // Note: This was checked in the datasheet for broadcom BCM2835, and not BCM2837 which is what is actually mounted on the 3b unit we have.
            _lidarPin = OpenNewPowerPin(gpioNumber: 12, equipmentName: "Lidar");
            _ultrasoundPin = OpenNewPowerPin(gpioNumber: 16, equipmentName:"Ultrasound");
            _wheelsPin = OpenNewPowerPin(gpioNumber: 20, equipmentName: "Wheels");
            _encoderPin = OpenNewPowerPin(gpioNumber: 21, equipmentName: "Encoder");

            _spare1Pin = OpenNewPowerPin(gpioNumber: 13, equipmentName: "Spare1");
            _spare2Pin = OpenNewPowerPin(gpioNumber: 19, equipmentName: "Spare2");
            _spare3Pin = OpenNewPowerPin(gpioNumber: 26, equipmentName: "Spare3");

            //TEMP: Issues with microcontrollers when not all are powered on. They are now physically connected to power. Sets the power switces as true on start up her, so that user does not have to do it manually.
            //TODO: Remove power for Ultrasuond, Wheels and Encoder from code implementation (or connect back the equipment physically to these powerpins)
            Ultrasound = true;
            Wheels = true;
            Encoder = true;
        }

        //TODO: Find a better way for exception handling of switching power state on pin (which requires less code repetition), and add exception handling for the remaining ones.
        private bool _lidar;
        public bool Lidar
        {
            get { return _lidar; }
            set
            {
                if (_lidarPin == null)
                {
                    RaiseSyncedPropertyChanged();
                    HasUnacknowledgedError = true;
                    if(value) Message += "Can's switch on power to Lidar, since its Power-pin did not open properly.\n** ** ** ** **\n";
                    return;
                }
                try
                {
                    _lidarPin.Power = value;
                    SetProperty(ref _lidar, value);
                }
                catch (Exception e)
                {
                    Message += $"LIDAR POWER ERROR:\n{e}\n\n{e.InnerException}\n** ** ** ** **\n";
                }
            }
        }

        private bool _ultrasound;
        public bool Ultrasound
        {
            get { return _ultrasound; }
            set
            {
                _ultrasoundPin.Power = value;
                SetProperty(ref _ultrasound, value);
            }
        }

        private bool _wheels;
        public bool Wheels
        {
            get { return _wheels; }
            set
            {
                _wheelsPin.Power = value;
                SetProperty(ref _wheels, value);
            }
        }

        private bool _encoder;
        public bool Encoder
        {
            get { return _encoder; }
            set
            {
                _encoderPin.Power = value;
                SetProperty(ref _encoder, value);
            }
        }

        private bool _spare1;
        public bool Spare1
        {
            get { return _spare1; }
            set
            {
                _spare1Pin.Power = value;
                SetProperty(ref _spare1, value);
            }
        }

        private bool _spare2;
        public bool Spare2
        {
            get { return _spare2; }
            set
            {
                _spare2Pin.Power = value;
                SetProperty(ref _spare2, value);
            }
        }

        private bool _spare3;
        public bool Spare3
        {
            get { return _spare3; }
            set
            {
                _spare3Pin.Power = value;
                SetProperty(ref _spare3, value);
            }
        }

        private bool _hasUnacknowledgedError;
        public bool HasUnacknowledgedError
        {
            get { return _hasUnacknowledgedError; }
            set { SetProperty(ref _hasUnacknowledgedError, value); }
        }

        private string _message;
        public string Message
        {
            get { return _message; }
            private set { SetProperty(ref _message, value); }
        }

        public void ClearMessage()
        {
            Message = "";
            HasUnacknowledgedError = false;
        }

        private PowerPin OpenNewPowerPin(int gpioNumber, string equipmentName)
        {
            try
            {
                return new PowerPin(gpioNumber);
            }
            catch (Exception e)
            {
                HasUnacknowledgedError = true;
                Message += $"PIN FAILED TO OPEN for {equipmentName}. Check connection for GPIO-pin {gpioNumber} and restart program.\n" +
                           $"If no loose connection was found, try restarting the operating system.\n** ** ** ** **\n";

                Debug.Print($"FAILED TO OPEN PIN {gpioNumber} \nException message:\n{e.Message} \n\n{e.InnerException}");

                return null;
            }
        }
    }
}
