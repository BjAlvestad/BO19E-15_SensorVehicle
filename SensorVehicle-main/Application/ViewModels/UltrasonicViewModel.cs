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
        public UltrasonicViewModel(IUltrasonic ultrasonic)
        {
            Ultrasonic = ultrasonic;
        }

        public IUltrasonic Ultrasonic { get; set; }
    }
}
