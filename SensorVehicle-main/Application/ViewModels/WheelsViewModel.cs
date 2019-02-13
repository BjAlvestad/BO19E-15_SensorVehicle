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

        public WheelsViewModel(IWheel wheel, IEncoder encoder)
        {
            Wheel = wheel;
            Encoder = encoder;
        }

        public IWheel Wheel { get; set; }

        public IEncoder Encoder { get; private set; }

        private TimeSpan _updateInterval;
        public TimeSpan UpdateInterval
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
            RaisePropertyChanged(nameof(Wheel));
        }

        public void StopWheels()
        {
            LeftWheel = 0;
            RightWheel = 0;
            Wheel.SetSpeed(LeftWheel, RightWheel);
        }

        private bool _applyWheelSpeedContinously;
        public bool ApplyWheelSpeedContinously
        {
            get { return _applyWheelSpeedContinously; }
            set
            {
                SetProperty(ref _applyWheelSpeedContinously, value);
                if (TogglePeriodicRaisePropertyChanged == false) TogglePeriodicRaisePropertyChanged = true;
            }
        }

        private bool _collectEncoderDistanceContinously;
        public bool CollectEncoderDistanceContinously
        {
            get { return _collectEncoderDistanceContinously; }
            set
            {
                SetProperty(ref _collectEncoderDistanceContinously, value);
                if (TogglePeriodicRaisePropertyChanged == false) TogglePeriodicRaisePropertyChanged = true;
            }
        }

        private bool _togglePeriodicRaisePropertyChanged;
        public bool TogglePeriodicRaisePropertyChanged
        {
            get { return _togglePeriodicRaisePropertyChanged; }
            set
            {
                bool valueChanged = SetProperty(ref _togglePeriodicRaisePropertyChanged, value);
                if (value && valueChanged)
                {
                    _periodicRaisePropertyChangedToken = new CancellationTokenSource();
                    PeriodicRaisePropertyChangedAsync(_periodicRaisePropertyChangedToken.Token);
                }
                else if(valueChanged)
                {
                    _periodicRaisePropertyChangedToken.Cancel();
                }
            }
        }

        private async Task PeriodicRaisePropertyChangedAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                if (ApplyWheelSpeedContinously)
                {
                    Wheel.SetSpeed(LeftWheel, RightWheel);
                }

                if (CollectEncoderDistanceContinously)
                {
                    Encoder.CollectAndResetDistanceFromEncoder();
                }

                RaisePropertyChanged(nameof(Wheel));
                RaisePropertyChanged(nameof(Encoder));
                await Task.Delay(UpdateInterval, cancellationToken);
            }
        }

        public override void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
        {
            base.OnNavigatedTo(e, viewModelState);
            LeftWheel = Wheel.CurrentSpeedLeft;
            RightWheel = Wheel.CurrentSpeedRight;
            UpdateInterval = TimeSpan.FromMilliseconds(300);
        }

        public override void OnNavigatingFrom(NavigatingFromEventArgs e, Dictionary<string, object> viewModelState, bool suspending)
        {
            ApplyWheelSpeedContinously = false;
            _periodicRaisePropertyChangedToken.Cancel();
            base.OnNavigatingFrom(e, viewModelState, suspending);
        }
    }
}
