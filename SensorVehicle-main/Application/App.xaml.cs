using System;
using System.Globalization;
using System.Threading.Tasks;

using Application.Views;

using Microsoft.Practices.Unity;

using Prism.Mvvm;
using Prism.Unity.Windows;
using Prism.Windows.AppModel;
using Prism.Windows.Navigation;

using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Resources;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Communication;
using Communication.MockCommunication;
using Communication.Simulator;
using Communication.Vehicle;
using ExampleLogic;
using StudentLogic;
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
        private readonly Uri _simulatorUri = new Uri("hvl-sensorvehicle-simulator:");  // Launch arguments may be put behind the colon

        // The public properties can be accessed from elsewhere in code using: ((App) PrismUnityApplication.Current).DesiredPropertyNameHere
        public bool IsRunningOnPhysicalCar { get; }
        public bool RunAgainstSimulatorInsteadOfMock { get; }
        public LaunchQuerySupportStatus SimulatorAppAvailabilityStatus { get; }

        private readonly SimulatorAppServiceClient _simulatorAppServiceClient = new SimulatorAppServiceClient();

        public App()
        {
            InitializeComponent();

            IsRunningOnPhysicalCar = Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.IoT";
            SimulatorAppAvailabilityStatus = Launcher.QueryUriSupportAsync(_simulatorUri, LaunchQuerySupportType.Uri).GetAwaiter().GetResult();
            RunAgainstSimulatorInsteadOfMock = SimulatorAppAvailabilityStatus == LaunchQuerySupportStatus.Available;
        }

        protected override void ConfigureContainer()
        {
            // register a singleton using Container.RegisterType<IInterface, Type>(new ContainerControlledLifetimeManager());
            base.ConfigureContainer();
            Container.RegisterInstance<IResourceLoader>(new ResourceLoaderAdapter(new ResourceLoader()));
            Container.RegisterType<ExampleLogicService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<StudentLogicService>(new ContainerControlledLifetimeManager());

            ILidarPacketReceiver lidarPacketReceiver;
            IVehicleCommunication ultrasonicCommunication;
            IVehicleCommunication encoderLeftCommunication;
            IVehicleCommunication encoderRightCommunication;
            IVehicleCommunication wheelCommunication;
            if (IsRunningOnPhysicalCar)
            {
                lidarPacketReceiver = new LidarPacketReceiver();
                ultrasonicCommunication = new VehicleCommunication(Device.Ultrasonic);
                encoderLeftCommunication = new VehicleCommunication(Device.EncoderLeft);
                encoderRightCommunication = new VehicleCommunication(Device.EncoderRight);
                wheelCommunication = new VehicleCommunication(Device.Wheel);
                Container.RegisterType<IPower, Power>(new ContainerControlledLifetimeManager());
            }
            else if (RunAgainstSimulatorInsteadOfMock)
            {
                lidarPacketReceiver = new SimulatedLidarPacketReceiver(_simulatorAppServiceClient);
                ultrasonicCommunication = new SimulatedVehicleCommunication(Device.Ultrasonic, _simulatorAppServiceClient);
                encoderLeftCommunication = new SimulatedVehicleCommunication(Device.EncoderLeft, _simulatorAppServiceClient);
                encoderRightCommunication = new SimulatedVehicleCommunication(Device.EncoderRight, _simulatorAppServiceClient);
                wheelCommunication = new SimulatedVehicleCommunication(Device.Wheel, _simulatorAppServiceClient);
                Container.RegisterType<IPower, SimulatedPower>(new ContainerControlledLifetimeManager());
            }
            else // Connect up against mock/random data instead of simulator
            {
                lidarPacketReceiver = new MockLidarPacketReceiver();
                ultrasonicCommunication = new MockVehicleCommunication(Device.Ultrasonic);
                encoderLeftCommunication = new MockVehicleCommunication(Device.EncoderLeft);
                encoderRightCommunication = new MockVehicleCommunication(Device.EncoderRight);
                wheelCommunication = new MockVehicleCommunication(Device.Wheel);
                Container.RegisterType<IPower, MockPower>(new ContainerControlledLifetimeManager());
            }

            Container.RegisterType<ILidarDistance, LidarDistance>(new ContainerControlledLifetimeManager(), new InjectionConstructor(lidarPacketReceiver, new VerticalAngle[] { VerticalAngle.Up1, VerticalAngle.Up3 }));
            Container.RegisterType<IUltrasonic, Ultrasonic>(new ContainerControlledLifetimeManager(), new InjectionConstructor(ultrasonicCommunication));
            Container.RegisterType<IEncoders, Encoders>(new ContainerControlledLifetimeManager(), new InjectionConstructor(new Encoder(encoderLeftCommunication), new Encoder(encoderRightCommunication)));
            Container.RegisterType<IWheel, Wheel>(new ContainerControlledLifetimeManager(), new InjectionConstructor(wheelCommunication));
        }

        protected override async Task OnLaunchApplicationAsync(LaunchActivatedEventArgs args)
        {
            await LaunchApplicationAsync(PageTokens.InfoPage, null);
            if (RunAgainstSimulatorInsteadOfMock && !IsRunningOnPhysicalCar)
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
            var success = await Launcher.LaunchUriAsync(_simulatorUri);
            return success;
        }
    }
}
