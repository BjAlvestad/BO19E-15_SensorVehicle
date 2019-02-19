using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Communication.Annotations;

namespace Communication.Vehicle
{
    public class Power : INotifyPropertyChanged, IPower
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
            _lidarPin = new PowerPin(12);
            _ultrasoundPin = new PowerPin(16);
            _wheelsPin = new PowerPin(20);
            _encoderPin = new PowerPin(21);

            _spare1Pin = new PowerPin(13);
            _spare2Pin = new PowerPin(19);
            _spare3Pin = new PowerPin(26);
        }

        private bool _lidar;
        public bool Lidar
        {
            get { return _lidar; }
            set
            {
                _lidar = value;
                _lidarPin.Power = value;
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
                _ultrasoundPin.Power = value;
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
                _wheelsPin.Power = value;
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
                _encoderPin.Power = value;
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
                _spare1Pin.Power = value;
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
                _spare2Pin.Power = value;
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
                _spare3Pin.Power = value;
                OnPropertyChanged(nameof(Spare3));
            }
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
