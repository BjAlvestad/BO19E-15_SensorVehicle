using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Data;
using Communication;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;
using VehicleEquipment.DistanceMeasurement.Lidar;

namespace Application.ViewModels
{
    public class LidarViewModel : ViewModelBase
    {
        public LidarViewModel(ILidarDistance lidar, IPower power)
        {
            Lidar = lidar;
            Power = power;
            FromAngle = 255;
            ToAngle = 285;
            CalculationTypes = new List<CalculationType>(Enum.GetValues(typeof(CalculationType)).Cast<CalculationType>());
            VerticalAngles = new List<VerticalAngle>(Enum.GetValues(typeof(VerticalAngle)).Cast<VerticalAngle>());
            ActiveVerticalAngles = new List<VerticalAngle>();
        }

        public List<VerticalAngle> ActiveVerticalAngles;

        public ILidarDistance Lidar { get; }

        public IPower Power { get; }

        public List<CalculationType> CalculationTypes { get; }

        private List<VerticalAngle> _verticalAngles;
        public List<VerticalAngle> VerticalAngles
        {
            get { return _verticalAngles; }
            set { SetProperty(ref _verticalAngles, value); }
        }

        public VerticalAngle SelectedVerticalAngle { get; set; }

        public VerticalAngle SelectedActiveVerticalAngle
        {
            get { return _selectedActiveVerticalAngle; }
            set { SetProperty(ref _selectedActiveVerticalAngle, value); }
        }

        public void AddSelectedVerticalAngleToActive()
        {
            Lidar.ActiveVerticalAngles.Add(SelectedVerticalAngle);
        }

        public void RemoveSelectedVerticalAngleFromActive()
        {
            if (Lidar.ActiveVerticalAngles.Count > 1 && SelectedActiveVerticalAngle != Lidar.DefaultVerticalAngle)
            {
                VerticalAngle angleToRemove = SelectedActiveVerticalAngle;
                SelectedActiveVerticalAngle = Lidar.DefaultVerticalAngle;
                Lidar.ActiveVerticalAngles.Remove(angleToRemove);
            }
        }

        public void SetAsDefaultAngle()
        {
            Lidar.DefaultVerticalAngle = SelectedActiveVerticalAngle;
        }

        private int _fromAngle;
        public int FromAngle
        {
            get { return _fromAngle; }
            set { SetProperty(ref _fromAngle, value); }
        }

        private int _toAngle;
        private VerticalAngle _selectedActiveVerticalAngle;

        public int ToAngle
        {
            get { return _toAngle; }
            set { SetProperty(ref _toAngle, value); }
        }

        public List<HorizontalPoint> HorizontalPointsInRange => Lidar.GetHorizontalPointsInRange(FromAngle, ToAngle, Lidar.DefaultVerticalAngle);

        private bool _autocalculate;
        public bool Autocalculate
        {
            get { return _autocalculate; }
            set { SetProperty(ref _autocalculate, value); }
        }

        private void Lidar_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(Lidar.LastUpdate)) return;

            RaisePropertyChanged(nameof(HorizontalPointsInRange));
            if (!Autocalculate) return;

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
