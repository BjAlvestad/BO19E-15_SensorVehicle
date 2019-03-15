using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Communication;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;
using VehicleEquipment.DistanceMeasurement.Ultrasound;

namespace Application.ViewModels
{
    public class UltrasonicViewModel : ViewModelBase
    {
        public UltrasonicViewModel(IUltrasonic ultrasonic, IPower power)
        {
            Ultrasonic = ultrasonic;
            Power = power;
        }

        public IUltrasonic Ultrasonic { get; set; }
        public IPower Power { get; set; }

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

        //TEMP: This property should be removed once the Microcontroller transmitts data after new measurements are taken (instead of the current method where new distance data must be requested before it will send new data)
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
                            float fwdValue = Ultrasonic.Fwd;  // Reading a value causes new data to be collected and all distance measurements updated
                            Thread.Sleep(Ultrasonic.PermissableDistanceAge / 2);
                        }
                    });
                }
            }
        }

        public override void OnNavigatingFrom(NavigatingFromEventArgs e, Dictionary<string, object> viewModelState, bool suspending)
        {
            RefreshUltrasonicContinously = false;
            base.OnNavigatingFrom(e, viewModelState, suspending);
        }
    }
}
