using System;
using System.Linq;
using System.Windows.Input;

using Application.Helpers;

using Prism.Commands;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

using Communication;
using ExampleLogic;
using VehicleEquipment.DistanceMeasurement.Lidar;
using VehicleEquipment.DistanceMeasurement.Ultrasound;
using VehicleEquipment.Locomotion.Encoder;
using VehicleEquipment.Locomotion.Wheels;

using WinUI = Microsoft.UI.Xaml.Controls;

namespace Application.ViewModels
{
    public class ShellViewModel : ViewModelBase
    {
        private static INavigationService _navigationService;
        private WinUI.NavigationView _navigationView;
        private bool _isBackEnabled;
        private WinUI.NavigationViewItem _selected;

        #region ControlLogicPropertiesForVisibilityBinding
        //TODO: Add StudentLogic here (and perform required changes in ShellPage) once that class/library has been properly implemented
        public ExampleLogicService ExampleLogic { get; }
        #endregion

        #region SensorPropertiesForColorBinding
        public ILidarDistance Lidar { get; }
        public IUltrasonic Ultrasonic { get; }
        public IWheel Wheel { get; }
        public IEncoder Encoder { get; }
        public IPower Power { get; }
        #endregion

        public ICommand ItemInvokedCommand { get; }

        public bool IsBackEnabled
        {
            get { return _isBackEnabled; }
            set { SetProperty(ref _isBackEnabled, value); }
        }

        public WinUI.NavigationViewItem Selected
        {
            get { return _selected; }
            set { SetProperty(ref _selected, value); }
        }

        public ShellViewModel(INavigationService navigationServiceInstance, ILidarDistance lidar, IUltrasonic ultrasonic, IWheel wheel, IEncoder encoder, IPower power, ExampleLogicService exampleLogic)
        {
            _navigationService = navigationServiceInstance;
            Lidar = lidar;
            Ultrasonic = ultrasonic;
            Wheel = wheel;
            Encoder = encoder;
            Power = power;
            ExampleLogic = exampleLogic;
            ItemInvokedCommand = new DelegateCommand<WinUI.NavigationViewItemInvokedEventArgs>(OnItemInvoked);
        }

        public void Initialize(Frame frame, WinUI.NavigationView navigationView)
        {
            _navigationView = navigationView;
            frame.NavigationFailed += (sender, e) =>
            {
                throw e.Exception;
            };
            frame.Navigated += Frame_Navigated;
            _navigationView.BackRequested += OnBackRequested;
        }

        private void OnItemInvoked(WinUI.NavigationViewItemInvokedEventArgs args)
        {
            var item = _navigationView.MenuItems
                            .OfType<WinUI.NavigationViewItem>()
                            .First(menuItem => (string)menuItem.Content == (string)args.InvokedItem);
            var pageKey = item.GetValue(NavHelper.NavigateToProperty) as string;
            _navigationService.Navigate(pageKey, null);
        }

        private void Frame_Navigated(object sender, NavigationEventArgs e)
        {
            IsBackEnabled = _navigationService.CanGoBack();
            Selected = _navigationView.MenuItems
                            .OfType<WinUI.NavigationViewItem>()
                            .FirstOrDefault(menuItem => IsMenuItemForPageType(menuItem, e.SourcePageType));
        }

        private void OnBackRequested(WinUI.NavigationView sender, WinUI.NavigationViewBackRequestedEventArgs args)
        {
            _navigationService.GoBack();
        }

        private bool IsMenuItemForPageType(WinUI.NavigationViewItem menuItem, Type sourcePageType)
        {
            var sourcePageKey = sourcePageType.Name;
            sourcePageKey = sourcePageKey.Substring(0, sourcePageKey.Length - 4);
            var pageKey = menuItem.GetValue(NavHelper.NavigateToProperty) as string;
            return pageKey == sourcePageKey;
        }
    }
}
