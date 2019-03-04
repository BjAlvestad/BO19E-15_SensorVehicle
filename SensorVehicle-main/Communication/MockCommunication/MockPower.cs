using System;
using System.Runtime.CompilerServices;
using Helpers;

namespace Communication.MockCommunication
{
    public class MockPower : ThreadSafeNotifyPropertyChanged, IPower
    {
        private bool _lidar;
        public bool Lidar
        {
            get => _lidar;
            set => SetProperty(ref _lidar, value);
        }

        private bool _ultrasound;
        public bool Ultrasound
        {
            get => _ultrasound;
            set => SetProperty(ref _ultrasound, value);
        }

        private bool _wheels;
        public bool Wheels
        {
            get => _wheels;
            set => SetProperty(ref _wheels, value);
        }

        private bool _encoder;
        public bool Encoder
        {
            get => _encoder;
            set => SetProperty(ref _encoder, value);
        }

        private bool _spare1;
        public bool Spare1
        {
            get => _spare1;
            set => SetProperty(ref _spare1, value);
        }

        private bool _spare2;
        public bool Spare2
        {
            get => _spare2;
            set => SetProperty(ref _spare2, value);
        }

        private bool _spare3;
        public bool Spare3
        {
            get => _spare3;
            set => SetProperty(ref _spare3, value);
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
        private bool SetPropertyWithErrorHandling(ref bool storage, bool value, [CallerMemberName] string propertyName = null)
        {
            try
            {
                return SetProperty(ref storage, value, propertyName);
            }
            catch (Exception e)
            {
                Message += $"An error occured when trying to switch {propertyName} {(value ? "ON" : "OFF")}:\n{e.Message}\n\nDetails: \n{e}\n****************\n\n";
                return false;
            }
        }
    }
}
