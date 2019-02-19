using System;
using Windows.ApplicationModel.Core;
using Windows.System;
using Windows.UI.Xaml;
using Communication;
using Prism.Windows.Mvvm;

namespace Application.ViewModels
{
    public class PowerViewModel : ViewModelBase
    {
        public PowerViewModel(IPower power)
        {
            Power = power;
        }

        private IPower _power;
        public IPower Power
        {
            get { return _power; }
            set { SetProperty(ref _power, value); }
        }

        public void PowerDownAllPins()
        {
            Power.Lidar = false;
            Power.Ultrasound = false;
            Power.Wheels = false;
            Power.Encoder = false;
            Power.Spare1 = false;
            Power.Spare2 = false;
            Power.Spare3 = false;
            RaisePropertyChanged(nameof(Power));
        }

        public Visibility VisibleIfRunningOnIoT => Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.IoT" ? Visibility.Visible : Visibility.Collapsed;

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
