﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using Prism.Commands;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using StudentLogic;
using VehicleEquipment.DistanceMeasurement.Lidar;
using VehicleEquipment.DistanceMeasurement.Ultrasound;
using VehicleEquipment.Locomotion.Wheels;

namespace Application.ViewModels
{
    public class StudentLogicViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly StudentLogicService _studentLogicService;
        private readonly IWheel _wheel;
        private readonly ILidarDistance _lidarDistance;
        private readonly IUltrasonic _ultrasonic;

        public StudentLogicViewModel(INavigationService navigationServiceInstance, StudentLogicService studentLogicServiceInstance, IWheel wheel, ILidarDistance lidarDistance, IUltrasonic ultrasonic)
        {
            _navigationService = navigationServiceInstance;
            _studentLogicService = studentLogicServiceInstance;
            StudentLogics = studentLogicServiceInstance.StudentLogics;
            _wheel = wheel;
            _lidarDistance = lidarDistance;
            _ultrasonic = ultrasonic;
        }

        private StudentLogicBase _selected;

        public StudentLogicBase Selected
        {
            get => _selected;
            set
            {
                if (Selected == null || Selected.RunStudentLogic == false)
                {
                    SetProperty(ref _selected, value);
                    _studentLogicService.ActiveStudentLogic = value;
                }
                else
                {
                    RaisePropertyChanged(nameof(Selected));
                }
            }
        }

        public ObservableCollection<StudentLogicBase> StudentLogics { get; }

        public override async void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
        {
            base.OnNavigatedTo(e, viewModelState);
        }

        public void SetDefaultSelection()
        {
            Selected = _studentLogicService.ActiveStudentLogic ?? StudentLogics.FirstOrDefault();
        }
    }
}
