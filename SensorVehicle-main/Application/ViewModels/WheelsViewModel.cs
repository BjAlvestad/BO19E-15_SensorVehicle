using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;
using VehicleEquipment.Locomotion.Encoder;
using VehicleEquipment.Locomotion.Wheels;

namespace Application.ViewModels
{
    public class WheelsViewModel : ViewModelBase
    {
        private CancellationTokenSource _periodicRaisePropertyChangedToken;

        public WheelsViewModel(IWheel wheel, IEncoders encoders)
        {
            Wheel = wheel;
            Encoders = encoders;
            UpdateInterval = 300;
        }

        public IWheel Wheel { get; set; }

        public IEncoders Encoders { get; private set; }

        public bool UnacknowledgedWheelOrEncoderError => Wheel.Error.Unacknowledged || Encoders.Error.Unacknowledged;

        private int _updateInterval;
        public int UpdateInterval
        {
            get { return _updateInterval; }
            set { SetProperty(ref _updateInterval, value); }
        }

        private int _leftWheel;
        public int LeftWheel
        {
            get { return _leftWheel; }
            set { SetProperty(ref _leftWheel, value); }
        }

        private int _rightWheel;
        public int RightWheel
        {
            get { return _rightWheel; }
            set { SetProperty(ref _rightWheel, value); }
        }

        public void ApplyNewWheelSpeed()
        {
            Wheel.SetSpeed(LeftWheel, RightWheel);
        }

        public void StopWheels()
        {
            if (ApplyWheelSpeedContinously)
            {
                LeftWheel = 0;
                RightWheel = 0;
            }
            Wheel.SetSpeed(0, 0, onlySendIfValuesChanged: false);
        }

        private bool _applyWheelSpeedContinously;
        public bool ApplyWheelSpeedContinously
        {
            get { return _applyWheelSpeedContinously; }
            set
            {
                bool valueChanged = SetProperty(ref _applyWheelSpeedContinously, value);
                if (value && valueChanged)
                {
                    _periodicRaisePropertyChangedToken = new CancellationTokenSource();
                    PeriodicApplyNewWheelSpeed(_periodicRaisePropertyChangedToken.Token);
                }
                else if(valueChanged)
                {
                    _periodicRaisePropertyChangedToken.Cancel();
                }
            }
        }

        //TODO: Consider removing PeriodicApplyNewWheelSpeed and simplifying ApplyWheelSpeedContinously to be a pure boolean. A method subscribed to the property changed notification of the sliders (properties they are bound to) can set the new speed IF ApplyWheelSpeedContinously is true. DateTime/TimeSpan can be used in the if-check to only raise at certain intervals if desired.
        private async Task PeriodicApplyNewWheelSpeed(CancellationToken cancellationToken)
        {
            while (true)
            {
                Wheel.SetSpeed(LeftWheel, RightWheel);

                await Task.Delay(UpdateInterval, cancellationToken);
            }
        }

        public override void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
        {
            base.OnNavigatedTo(e, viewModelState);
            LeftWheel = Wheel.CurrentSpeedLeft;
            RightWheel = Wheel.CurrentSpeedRight;
            Wheel.PropertyChanged += Wheel_PropertyChanged;
            Wheel.Error.PropertyChanged += WheelError_PropertyChanged;
            Encoders.Error.PropertyChanged += EncoderError_PropertyChanged;
            RaisePropertyChanged(nameof(UnacknowledgedWheelOrEncoderError));
        }

        public override void OnNavigatingFrom(NavigatingFromEventArgs e, Dictionary<string, object> viewModelState, bool suspending)
        {
            Wheel.PropertyChanged -= Wheel_PropertyChanged;
            Wheel.Error.PropertyChanged -= WheelError_PropertyChanged;
            Encoders.Error.PropertyChanged -= EncoderError_PropertyChanged;
            ApplyWheelSpeedContinously = false;
            _periodicRaisePropertyChangedToken?.Cancel();
            base.OnNavigatingFrom(e, viewModelState, suspending);
        }

        private void Wheel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Wheel.Power) when Wheel.Power == false:
                    ApplyWheelSpeedContinously = false;
                    break;
            }
        }

        private void EncoderError_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Encoders.Error.Unacknowledged))
            {
                RaisePropertyChanged(nameof(UnacknowledgedWheelOrEncoderError));
            }
        }

        private void WheelError_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Wheel.Error.Unacknowledged))
            {
                RaisePropertyChanged(nameof(UnacknowledgedWheelOrEncoderError));
            }
        }
    }
}
