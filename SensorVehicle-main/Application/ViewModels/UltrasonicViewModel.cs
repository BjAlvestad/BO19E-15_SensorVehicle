using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.Helpers;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;
using VehicleEquipment.DistanceMeasurement.Ultrasound;

namespace Application.ViewModels
{
    public class UltrasonicViewModel : ViewModelBase
    {
        public UltrasonicViewModel(IUltrasonic ultrasonic)
        {
            Ultrasonic = ultrasonic;
        }

        public IUltrasonic Ultrasonic { get; set; }

        public string PermissableDistanceAge
        {
            get { return Ultrasonic.PermissableDistanceAge.Milliseconds.ToString(); }
            set
            {
                if (Int32.TryParse(value, out int inputValue))
                {
                    int validInputValue = (inputValue > 1000) ? 999 : inputValue;
                    Ultrasonic.PermissableDistanceAge = TimeSpan.FromMilliseconds(validInputValue);
                }

                RaisePropertyChanged();  
            }
        }

        private bool _refreshUltrasonicContinously;
        public bool RefreshUltrasonicContinously
        {
            get { return _refreshUltrasonicContinously; }
            set
            {
                SetProperty(ref _refreshUltrasonicContinously, value);
                if (value)
                {
                    Task.Run(() =>
                    {
                        while (RefreshUltrasonicContinously)
                        {
                            DispatcherHelper.ExecuteOnUIThreadAsync(() => RaisePropertyChanged(nameof(Ultrasonic)));
                            Thread.Sleep(Ultrasonic.PermissableDistanceAge / 2);
                        }
                    });
                }
            }
        }

        public override void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
        {
            base.OnNavigatedTo(e, viewModelState);
            RefreshUltrasonicContinously = true;
        }

        public override void OnNavigatingFrom(NavigatingFromEventArgs e, Dictionary<string, object> viewModelState, bool suspending)
        {
            RefreshUltrasonicContinously = false;
            base.OnNavigatingFrom(e, viewModelState, suspending);
        }
    }
}
