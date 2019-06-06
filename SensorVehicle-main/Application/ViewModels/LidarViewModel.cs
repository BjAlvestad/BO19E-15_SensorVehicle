using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Communication;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;
using VehicleEquipment.DistanceMeasurement.Lidar;

namespace Application.ViewModels
{
    public class LidarViewModel : ViewModelBase
    {
        public LidarViewModel(ILidarDistance lidar)
        {
            Lidar = lidar;
            CenterForAnglesInRange = 0;
            BeamOpeningForAnglesInRange = 2;
            CalculationTypes = new List<CalculationType>(Enum.GetValues(typeof(CalculationType)).Cast<CalculationType>());
            VerticalAngles = new List<VerticalAngle>(Enum.GetValues(typeof(VerticalAngle)).Cast<VerticalAngle>());
            ActiveVerticalAngles = new List<VerticalAngle>();
        }

        public List<VerticalAngle> ActiveVerticalAngles;

        public ILidarDistance Lidar { get; }

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
            Lidar.Config.ActiveVerticalAngles.Add(SelectedVerticalAngle);
        }

        public void RemoveSelectedVerticalAngleFromActive()
        {
            if (Lidar.Config.ActiveVerticalAngles.Count > 1 && SelectedActiveVerticalAngle != Lidar.Config.DefaultVerticalAngle)
            {
                VerticalAngle angleToRemove = SelectedActiveVerticalAngle;
                SelectedActiveVerticalAngle = Lidar.Config.DefaultVerticalAngle;
                Lidar.Config.ActiveVerticalAngles.Remove(angleToRemove);
            }
        }

        public void SetAsDefaultAngle()
        {
            Lidar.Config.DefaultVerticalAngle = SelectedActiveVerticalAngle;
        }

        public int CenterForAnglesInRange { get; set; }
        public int BeamOpeningForAnglesInRange { get; set; }

        private VerticalAngle _selectedActiveVerticalAngle;

        private string _selectedAngleRange;
        public string SelectedAngleRange
        {
            get { return _selectedAngleRange; }
            set { SetProperty(ref _selectedAngleRange, value); }
        }

        private List<HorizontalPoint> _horizontalPointsInRange;
        public List<HorizontalPoint> HorizontalPointsInRange
        {
            get { return _horizontalPointsInRange; }
            set { SetProperty(ref _horizontalPointsInRange, value); }
        }

        private bool _autoCalculateDirections;
        public bool AutoCalculateDirections
        {
            get { return _autoCalculateDirections; }
            set { SetProperty(ref _autoCalculateDirections, value); }
        }

        private bool _autoCalculateLargestDistance;
        public bool AutoCalculateLargestDistance
        {
            get { return _autoCalculateLargestDistance; }
            set { SetProperty(ref _autoCalculateLargestDistance, value); }
        }

        private bool _calculateHorizontalPoints;
        public bool CalculateHorizontalPoints
        {
            get { return _calculateHorizontalPoints; }
            set { SetProperty(ref _calculateHorizontalPoints, value); }
        }

        private async void Lidar_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Lidar.RunCollector) when Lidar.RunCollector == false:
                    CalculateHorizontalPoints = false;
                    break;
                case nameof(Lidar.RaiseNotificationForSelective) when Lidar.RaiseNotificationForSelective == false:
                    AutoCalculateDirections = false;
                    AutoCalculateLargestDistance = false;
                    break;
            }

            if (e.PropertyName != nameof(Lidar.LastUpdate)) return;

            if (AutoCalculateDirections)
            {
                Task.Run(() =>
                {
                    float fwd = Lidar.Fwd;
                    float left = Lidar.Left;
                    float right = Lidar.Right;
                });
            }

            if (AutoCalculateLargestDistance)
            {
                HorizontalPoint point = Lidar.LargestDistance;
            }

            if (CalculateHorizontalPoints)
            {
                float fromAngle = CenterForAnglesInRange - BeamOpeningForAnglesInRange / 2;
                float toAngle = CenterForAnglesInRange + BeamOpeningForAnglesInRange / 2;
                if (fromAngle < 0) fromAngle += 360;
                if (toAngle > 360) fromAngle -= 360;

                SelectedAngleRange = $"From {fromAngle} to {toAngle}";
                HorizontalPointsInRange = await Task.Run(() => Lidar.GetHorizontalPointsInRange(fromAngle, toAngle, Lidar.Config.DefaultVerticalAngle));
            }
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
