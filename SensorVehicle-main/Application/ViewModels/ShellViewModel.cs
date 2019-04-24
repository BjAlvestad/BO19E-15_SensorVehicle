using System;
using System.Linq;
using System.Windows.Input;

using Application.Helpers;
using Application.Views;

using Prism.Commands;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

using Communication;
using ExampleLogic;
using StudentLogic;
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
        public ExampleLogicService ExampleLogic { get; }
        public StudentLogicService StudentLogic { get; }
        #endregion

        #region SensorPropertiesForColorBinding
        public ILidarDistance Lidar { get; }
        public IUltrasonic Ultrasonic { get; }
        public IWheel Wheel { get; }
        public IEncoders Encoders { get; }

        public bool UnacknowledgedWheelOrEncoderError => Wheel.Error.Unacknowledged || Encoders.Error.Unacknowledged;
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

        public ShellViewModel(INavigationService navigationServiceInstance, ILidarDistance lidar, IUltrasonic ultrasonic, IWheel wheel, IEncoders encoders, ExampleLogicService exampleLogic, StudentLogicService studentLogic)
        {
            _navigationService = navigationServiceInstance;
            Lidar = lidar;
            Ultrasonic = ultrasonic;
            Wheel = wheel;
            Encoders = encoders;
            ExampleLogic = exampleLogic;
            StudentLogic = studentLogic;
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
            Wheel.Error.PropertyChanged += WheelError_PropertyChanged;
            Encoders.Error.PropertyChanged += EncoderError_PropertyChanged;
        }

        private void OnItemInvoked(WinUI.NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                _navigationService.Navigate("Settings", null);
                return;
            }

            var item = _navigationView.MenuItems
                            .OfType<WinUI.NavigationViewItem>()
                            .First(menuItem => (string)menuItem.Content == (string)args.InvokedItem);
            var pageKey = item.GetValue(NavHelper.NavigateToProperty) as string;
            _navigationService.Navigate(pageKey, null);
        }

        private void Frame_Navigated(object sender, NavigationEventArgs e)
        {
            IsBackEnabled = _navigationService.CanGoBack();
            if (e.SourcePageType == typeof(SettingsPage))
            {
                Selected = _navigationView.SettingsItem as WinUI.NavigationViewItem;
                return;
            }

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

        #region WheelAndEncoderErrorCombiner
        private void EncoderError_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Encoders.Error.Unacknowledged))
            {
                RaisePropertyChanged(nameof(UnacknowledgedWheelOrEncoderError));
            }
        }

        private void WheelError_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Wheel.Error.Unacknowledged))
            {
                RaisePropertyChanged(nameof(UnacknowledgedWheelOrEncoderError));
            }
        }
        #endregion
    }
}
