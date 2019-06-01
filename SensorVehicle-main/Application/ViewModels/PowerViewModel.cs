using System;
using Windows.ApplicationModel.Core;
using Windows.System;
using Windows.UI.Xaml;
using Application.Helpers;
using Communication;
using Prism.Unity.Windows;
using Prism.Windows.Mvvm;
using VehicleEquipment.DistanceMeasurement.Lidar;
using VehicleEquipment.DistanceMeasurement.Ultrasound;
using VehicleEquipment.Locomotion.Encoder;
using VehicleEquipment.Locomotion.Wheels;

namespace Application.ViewModels
{
    public class PowerViewModel : ViewModelBase
    {
        public PowerViewModel(ILidarDistance lidar, IWheel wheel, IEncoders encoders, IUltrasonic ultrasonic)
        {
            Lidar = lidar;
            Wheel = wheel;
            Encoders = encoders;
            Ultrasonic = ultrasonic;
        }

        public ILidarDistance Lidar { get; }
        public IWheel Wheel { get; }
        public IEncoders Encoders { get; }
        public IUltrasonic Ultrasonic { get; }

        public void PowerDownAllPins()
        {
            Lidar.Power = false;
            Wheel.Power = false;
            Encoders.Power = false;
            Ultrasonic.Power = false;
        }

        public Visibility VisibleIfRunningOnIoT => ((App) PrismUnityApplication.Current).ProgramRunningState == RunningState.OnPhysicalCar ? Visibility.Visible : Visibility.Collapsed;

        public void ExitApplication()
        {
            CoreApplication.Exit();
        }

        public void RestartSystem()
        {
            ShutdownManager.BeginShutdown(ShutdownKind.Restart, TimeSpan.FromSeconds(0));
        }

        public void ShutDownSystem()
        {
            PowerDownAllPins();
            ShutdownManager.BeginShutdown(ShutdownKind.Shutdown, TimeSpan.FromSeconds(0));
        }
    }
}
