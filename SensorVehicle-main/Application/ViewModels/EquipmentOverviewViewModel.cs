using System;
using System.Collections.Generic;
using Communication;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;
using VehicleEquipment.DistanceMeasurement.Lidar;
using VehicleEquipment.DistanceMeasurement.Ultrasound;
using VehicleEquipment.Locomotion.Encoder;
using VehicleEquipment.Locomotion.Wheels;

namespace Application.ViewModels
{
    public class EquipmentOverviewViewModel : ViewModelBase
    {
        public EquipmentOverviewViewModel(IUltrasonic ultrasonic, ILidarDistance lidar, IWheel wheel, IEncoder encoder, IPower power)
        {
            Ultrasonic = ultrasonic;
            Lidar = lidar;
            Wheel = wheel;
            Encoder = encoder;
            Power = power;
        }

        public IUltrasonic Ultrasonic { get; set; }
        public ILidarDistance Lidar { get; set; }
        public IWheel Wheel { get; set; }
        public IEncoder Encoder { get; set; }
        public IPower Power { get; set; }

        private bool _autoCalculateLidarDistance;
        public bool AutoCalculateLidarDistance
        {
            get { return _autoCalculateLidarDistance; }
            set { SetProperty(ref _autoCalculateLidarDistance, value); }
        }

        private void Lidar_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(Lidar.LastUpdate)) return;

            if (!AutoCalculateLidarDistance) return;

            float fwd = Lidar.Fwd;
            float left = Lidar.Left;
            float right = Lidar.Right;
        }

        public override void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
        {
            base.OnNavigatedTo(e, viewModelState);
            Lidar.PropertyChanged += Lidar_PropertyChanged;
        }

        public override void OnNavigatingFrom(NavigatingFromEventArgs e, Dictionary<string, object> viewModelState, bool suspending)
        {
            Lidar.PropertyChanged -= Lidar_PropertyChanged;
            base.OnNavigatingFrom(e, viewModelState, suspending);
        }
    }
}
