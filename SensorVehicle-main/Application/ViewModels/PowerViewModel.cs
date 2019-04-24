using System;
using Windows.ApplicationModel.Core;
using Windows.System;
using Windows.UI.Xaml;
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
        public PowerViewModel(ILidarDistance lidarDistance, IWheel wheel, IEncoders encoders, IUltrasonic ultrasonic)
        {
            Lidar = lidarDistance;
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
            Ultrasonic.DeisolateI2cCommunciation = false;
        }

        public Visibility VisibleIfRunningOnIoT => ((App) PrismUnityApplication.Current).IsRunningOnPhysicalCar ? Visibility.Visible : Visibility.Collapsed;

        public void ExitApplication()
        {
            CoreApplication.Exit();
        }

        public void RestartSystem()
        {
            // Restarts the device within 5 seconds:
            ShutdownManager.BeginShutdown(ShutdownKind.Restart, TimeSpan.FromSeconds(5));
        }

        public void ShutDownSystem()
        {
            PowerDownAllPins();
            ShutdownManager.BeginShutdown(ShutdownKind.Shutdown, TimeSpan.FromSeconds(0));
        }
    }
}
