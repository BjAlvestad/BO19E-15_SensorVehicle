using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    }
}
