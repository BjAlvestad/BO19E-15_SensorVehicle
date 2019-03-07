using System;
using System.Globalization;
using System.Threading.Tasks;

using Application.Core.Services;
using Application.Views;

using Microsoft.Practices.Unity;

using Prism.Mvvm;
using Prism.Unity.Windows;
using Prism.Windows.AppModel;
using Prism.Windows.Navigation;

using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Communication;
using Communication.MockCommunication;
using Communication.Simulator;
using Communication.Vehicle;
using ExampleLogic;
using VehicleEquipment;
using VehicleEquipment.DistanceMeasurement.Lidar;
using VehicleEquipment.DistanceMeasurement.Ultrasound;
using VehicleEquipment.Locomotion.Encoder;
using VehicleEquipment.Locomotion.Wheels;

namespace Application
{
    [Windows.UI.Xaml.Data.Bindable]
    public sealed partial class App : PrismUnityApplication
    {
        private const bool RunAgainstSimulatorInsteadOfMock = true;
        private readonly bool _isRunningOnPhysicalCar = Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.IoT";

        private readonly SimulatorAppServiceClient _simulatorAppServiceClient = new SimulatorAppServiceClient();

        public App()
        {
            InitializeComponent();
        }

        protected override void ConfigureContainer()
        {
            // register a singleton using Container.RegisterType<IInterface, Type>(new ContainerControlledLifetimeManager());
            base.ConfigureContainer();
            Container.RegisterInstance<IResourceLoader>(new ResourceLoaderAdapter(new ResourceLoader()));
            Container.RegisterType<ISampleDataService, SampleDataService>();  //TEMP: Remove after StudentLogic and ExampleLogic pages has been changed
            Container.RegisterType<ExampleLogicService>(new ContainerControlledLifetimeManager());

            ILidarPacketReceiver lidarPacketReceiver;
            IVehicleCommunication ultrasonicCommunication;
            IVehicleCommunication encoderCommunication;
            IVehicleCommunication wheelCommunication;
            if (_isRunningOnPhysicalCar)
            {
                lidarPacketReceiver = new LidarPacketReceiver();
                ultrasonicCommunication = new VehicleCommunication(Device.Ultrasonic);
                encoderCommunication = new VehicleCommunication(Device.EncoderLeft);
                wheelCommunication = new VehicleCommunication(Device.Wheel);
                Container.RegisterType<IPower, Power>(new ContainerControlledLifetimeManager());
            }
            else if (RunAgainstSimulatorInsteadOfMock)
            {
                lidarPacketReceiver = new SimulatedLidarPacketReceiver(_simulatorAppServiceClient);
                ultrasonicCommunication = new SimulatedVehicleCommunication(Device.Ultrasonic, _simulatorAppServiceClient);
                encoderCommunication = new SimulatedVehicleCommunication(Device.EncoderLeft, _simulatorAppServiceClient);
                wheelCommunication = new SimulatedVehicleCommunication(Device.Wheel, _simulatorAppServiceClient);
                Container.RegisterType<IPower, SimulatedPower>(new ContainerControlledLifetimeManager());
                // TODO: Configure for communication against simulator (after SimulatedVehicleEquipment class is created)
            }
            else // Connect up against mock/random data instead of simulator
#pragma warning disable 162
            {
                lidarPacketReceiver = new MockLidarPacketReceiver();
                ultrasonicCommunication = new MockVehicleCommunication(Device.Ultrasonic);
                encoderCommunication = new MockVehicleCommunication(Device.EncoderLeft);
                wheelCommunication = new MockVehicleCommunication(Device.Wheel);
                Container.RegisterType<IPower, MockPower>(new ContainerControlledLifetimeManager());
            }
#pragma warning restore 162

            Container.RegisterType<ILidarDistance, LidarDistance>(new ContainerControlledLifetimeManager(), new InjectionConstructor(lidarPacketReceiver, new VerticalAngle[] { VerticalAngle.Up1, VerticalAngle.Up3 }));
            Container.RegisterType<IUltrasonic, Ultrasonic>(new ContainerControlledLifetimeManager(), new InjectionConstructor(ultrasonicCommunication));
            Container.RegisterType<IEncoder, Encoder>(new ContainerControlledLifetimeManager(), new InjectionConstructor(encoderCommunication));
            Container.RegisterType<IWheel, Wheel>(new ContainerControlledLifetimeManager(), new InjectionConstructor(wheelCommunication));
        }

        protected override async Task OnLaunchApplicationAsync(LaunchActivatedEventArgs args)
        {
            await LaunchApplicationAsync(PageTokens.InfoPage, null);
            if (RunAgainstSimulatorInsteadOfMock && !_isRunningOnPhysicalCar)
            {
                await LaunchSimulator();
            }
        }

        private async Task LaunchApplicationAsync(string page, object launchParam)
        {
            NavigationService.Navigate(page, launchParam);
            Window.Current.Activate();
            await Task.CompletedTask;
        }

        protected override async Task OnActivateApplicationAsync(IActivatedEventArgs args)
        {
            await Task.CompletedTask;
        }

        protected override async Task OnInitializeAsync(IActivatedEventArgs args)
        {
            // We are remapping the default ViewNamePage and ViewNamePageViewModel naming to ViewNamePage and ViewNameViewModel to
            // gain better code reuse with other frameworks and pages within Windows Template Studio
            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver((viewType) =>
            {
                var viewModelTypeName = string.Format(CultureInfo.InvariantCulture, "Application.ViewModels.{0}ViewModel, Application", viewType.Name.Substring(0, viewType.Name.Length - 4));
                return Type.GetType(viewModelTypeName);
            });
            await base.OnInitializeAsync(args);
        }

        protected override IDeviceGestureService OnCreateDeviceGestureService()
        {
            var service = base.OnCreateDeviceGestureService();
            service.UseTitleBarBackButton = false;
            return service;
        }

        public void SetNavigationFrame(Frame frame)
        {
            var sessionStateService = Container.Resolve<ISessionStateService>();
            CreateNavigationService(new FrameFacadeAdapter(frame), sessionStateService);
        }

        protected override UIElement CreateShell(Frame rootFrame)
        {
            var shell = Container.Resolve<ShellPage>();
            shell.SetRootFrame(rootFrame);
            return shell;
        }

        private async Task<bool> LaunchSimulator()
        {
            var uri = new Uri("hvl-sensorvehicle-simulator:");  // launch arguments may be put behind the colon
            var success = await Windows.System.Launcher.LaunchUriAsync(uri);
            return success;
        }

        //TODO: Hvis denne ikke brukes, s[ slett den
        //private async Task EnsureConnected()
        //{
        //    var retryDelay = 10000;
        //    await Task.Delay(retryDelay);
        //    while (!_connection.IsConnected)
        //    {
        //        try
        //        {
        //            await _connection.Open();
        //        }
        //        catch (Exception)
        //        {
        //            // note ensure MessageRelay is deployed by right clicking on UwpMessageRelay.MessageRelay and selecting "Deploy"
        //            string MessageResultsText = $"Unable to connect to siren of shame engine. Retrying in {(retryDelay / 1000)} seconds...";
        //            await Task.Delay(retryDelay);
        //        }
        //    }
        //}
    }
}
