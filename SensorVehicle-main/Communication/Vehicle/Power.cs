using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Windows.Devices.Gpio;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Communication.Annotations;

namespace Communication.Vehicle
{
    public class Power : INotifyPropertyChanged, IPower
    {
        //TODO: Extract pin out to separate class
        private readonly GpioController _controller;
        private readonly GpioPin _lidarPin;
        private readonly GpioPin _ultrasoundPin;
        private readonly GpioPin _wheelsPin;
        private readonly GpioPin _encoderPin;
        private readonly GpioPin _spare1Pin;
        private readonly GpioPin _spare2Pin;
        private readonly GpioPin _spare3Pin;

        public Power()
        {
            _controller = GpioController.GetDefault();

            // TODO: Set pin numbers according to physical connections
            //_controller.TryOpenPin(29, GpioSharingMode.Exclusive, out _lidarPin, out LidarPinStatus);

            _lidarPin = _controller.OpenPin(5);  // NOTE: PinNumber is the GPIO number. E.g. 5 for GPIO5 (which resides on physical pin number 29)
            _lidarPin.SetDriveMode(GpioPinDriveMode.Output);
            _lidarPin.Write(GpioPinValue.Low);

            _ultrasoundPin = _controller.OpenPin(6);
            _ultrasoundPin.SetDriveMode(GpioPinDriveMode.Output);
            _ultrasoundPin.Write(GpioPinValue.Low);


            _wheelsPin = _controller.OpenPin(13);
            _wheelsPin.SetDriveMode(GpioPinDriveMode.Output);
            _wheelsPin.Write(GpioPinValue.Low);

            _encoderPin = _controller.OpenPin(26);
            _encoderPin.SetDriveMode(GpioPinDriveMode.Output);
            _encoderPin.Write(GpioPinValue.Low);

            _spare1Pin = _controller.OpenPin(23);
            _spare1Pin.SetDriveMode(GpioPinDriveMode.Output);
            _spare2Pin = _controller.OpenPin(24);
            _spare2Pin.SetDriveMode(GpioPinDriveMode.Output);
            _spare3Pin = _controller.OpenPin(25);
            _spare3Pin.SetDriveMode(GpioPinDriveMode.Output);
        }

        private bool _lidar;
        public bool Lidar
        {
            get { return _lidar; }
            set
            {
                _lidar = value; 
                SetPin(_lidarPin, value);
                OnPropertyChanged(nameof(Lidar));
            }
        }

        public bool LidarPinStatus { get; set; }

        private bool _ultrasound;
        public bool Ultrasound
        {
            get { return _ultrasound; }
            set
            {
                _ultrasound = value; 
                SetPin(_ultrasoundPin, value);
                OnPropertyChanged(nameof(Ultrasound));
            }
        }

        private bool _wheels;
        public bool Wheels
        {
            get { return _wheels; }
            set
            {
                _wheels = value; 
                SetPin(_wheelsPin, value);
                OnPropertyChanged(nameof(Wheels));
            }
        }

        private bool _encoder;
        public bool Encoder
        {
            get { return _encoder; }
            set
            {
                _encoder = value; 
                SetPin(_encoderPin, value);
                OnPropertyChanged(nameof(Encoder));
            }
        }

        private bool _spare1;
        public bool Spare1
        {
            get { return _spare1; }
            set
            {
                _spare1 = value; 
                SetPin(_spare1Pin, value);
                OnPropertyChanged(nameof(Spare1));
            }
        }

        private bool _spare2;
        public bool Spare2
        {
            get { return _spare2; }
            set
            {
                _spare2 = value; 
                SetPin(_spare2Pin, value);
                OnPropertyChanged(nameof(Spare2));
            }
        }

        private bool _spare3;
        public bool Spare3
        {
            get { return _spare3; }
            set
            {
                _spare3 = value; 
                SetPin(_spare3Pin, value);
                OnPropertyChanged(nameof(Spare3));
            }
        }

        private void SetPin(GpioPin pin, bool pinState)
        {
            pin.Write(pinState ? GpioPinValue.High : GpioPinValue.Low);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual async void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            await Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.High,
                () => { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); });
        }
    }
}
