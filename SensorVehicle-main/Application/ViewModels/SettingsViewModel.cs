using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

using Application.Helpers;
using Application.Services;

using Prism.Commands;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;

using Windows.ApplicationModel;
using Windows.System;
using Windows.UI.Xaml;

using Communication.ExternalCommunication.StreamSocketServer;
using VehicleEquipment.DistanceMeasurement.Lidar;
using VehicleEquipment.DistanceMeasurement.Ultrasound;
using VehicleEquipment.Locomotion.Encoder;
using VehicleEquipment.Locomotion.Wheels;

namespace Application.ViewModels
{
    // For help see https://github.com/Microsoft/WindowsTemplateStudio/blob/master/docs/pages/settings.md
    public class SettingsViewModel : ViewModelBase
    {
        public SocketServer AsyncSocketServer { get; }

        private ElementTheme _elementTheme = ThemeSelectorService.Theme;
        public ElementTheme ElementTheme
        {
            get { return _elementTheme; }

            set { SetProperty(ref _elementTheme, value); }
        }

        private string _versionDescription;
        public string VersionDescription
        {
            get { return _versionDescription; }

            set { SetProperty(ref _versionDescription, value); }
        }

        private ICommand _switchThemeCommand;
        public ICommand SwitchThemeCommand
        {
            get
            {
                if (_switchThemeCommand == null)
                {
                    _switchThemeCommand = new DelegateCommand<object>(
                        async (param) =>
                        {
                            ElementTheme = (ElementTheme)param;
                            await ThemeSelectorService.SetThemeAsync((ElementTheme)param);
                        });
                }

                return _switchThemeCommand;
            }
        }

        private bool _runAsyncSocketServer;
        public bool RunAsyncSocketServer
        {
            get { return _runAsyncSocketServer; }
            set
            {
                if (value == _runAsyncSocketServer) return;

                if (value)
                {
                    AsyncSocketServer.StartServer();
                }
                else
                {
                    AsyncSocketServer.StopServer();
                }

                SetProperty(ref _runAsyncSocketServer, value);
            }
        }

        public SettingsViewModel(IUltrasonic ultrasonic, ILidarDistance lidar, IWheel wheel, IEncoders encoders)
        {
            AsyncSocketServer = new SocketServer(wheel, ultrasonic, lidar, encoders);
        }

        public async Task LaunchExtraFunctions()
        {
            Uri extraFunctionsUri = new Uri("hvl-sensorvehicle-extras:");
            bool success = await Launcher.LaunchUriAsync(extraFunctionsUri);
        }

        public override void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
        {
            base.OnNavigatedTo(e, viewModelState);

            VersionDescription = GetVersionDescription();
        }

        private string GetVersionDescription()
        {
            var appName = "AppDisplayName".GetLocalized();
            var package = Package.Current;
            var packageId = package.Id;
            var version = packageId.Version;

            return $"{appName} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }
    }
}
