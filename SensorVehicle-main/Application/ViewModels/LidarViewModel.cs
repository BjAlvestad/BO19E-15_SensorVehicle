﻿using System;
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
        public LidarViewModel(ILidarDistance lidar, IPower power)
        {
            Lidar = lidar;
            Power = power;
            CenterForAnglesInRange = 0;
            BeamOpeningForAnglesInRange = 2;
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

        private bool _autocalculate;
        public bool Autocalculate
        {
            get { return _autocalculate; }
            set { SetProperty(ref _autocalculate, value); }
        }

        private bool _calculateHorizontalPoints;
        public bool CalculateHorizontalPoints
        {
            get { return _calculateHorizontalPoints; }
            set { SetProperty(ref _calculateHorizontalPoints, value); }
        }

        private async void Lidar_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(Lidar.LastUpdate)) return;

            if (Autocalculate)
            {
                Task.Run(() =>
                {
                    float fwd = Lidar.Fwd;
                    float left = Lidar.Left;
                    float right = Lidar.Right;
                });
            }

            if (CalculateHorizontalPoints)
            {
                float fromAngle = CenterForAnglesInRange - BeamOpeningForAnglesInRange / 2;
                float toAngle = CenterForAnglesInRange + BeamOpeningForAnglesInRange / 2;
                if (fromAngle < 0) fromAngle += 360;
                if (toAngle > 360) fromAngle -= 360;

                SelectedAngleRange = $"From {fromAngle} to {toAngle}";
                HorizontalPointsInRange = await Task.Run(() => Lidar.GetHorizontalPointsInRange(fromAngle, toAngle, Lidar.DefaultVerticalAngle));
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
