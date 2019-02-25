using System;
using Communication;
using Prism.Windows.Mvvm;
using VehicleEquipment.DistanceMeasurement.Lidar;

namespace Application.ViewModels
{
    public class LidarViewModel : ViewModelBase
    {
        public LidarViewModel(ILidarDistance lidar, IPower power)
        {
            Lidar = lidar;
            Power = power;
        }

        public ILidarDistance Lidar { get; private set; }

        public IPower Power { get; private set; }
    }
}
