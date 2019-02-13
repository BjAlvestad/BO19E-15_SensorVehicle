using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using Application.Core.Models;
using Application.Core.Services;

using Prism.Commands;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using ExampleLogic;
using VehicleEquipment.DistanceMeasurement.Lidar;
using VehicleEquipment.DistanceMeasurement.Ultrasound;
using VehicleEquipment.Locomotion.Wheels;

namespace Application.ViewModels
{
    public class ExampleLogicViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly ExampleLogicService _exampleLogicService;
        private readonly IWheel _wheel;
        private readonly ILidarDistance _lidarDistance;
        private readonly IUltrasonic _ultrasonic;

        public ExampleLogicViewModel(INavigationService navigationServiceInstance, ExampleLogicService exampleLogicService, IWheel wheel, ILidarDistance lidarDistance, IUltrasonic ultrasonic)
        {
            _navigationService = navigationServiceInstance;
            _exampleLogicService = exampleLogicService;
            _wheel = wheel;
            _lidarDistance = lidarDistance;
            _ultrasonic = ultrasonic;
        }

        private IExampleLogic _selected;
        public IExampleLogic Selected
        {
            get => _selected;
            set => SetProperty(ref _selected, value);
        }

        public ObservableCollection<IExampleLogic> ExamleLogics { get; } = new ObservableCollection<IExampleLogic>();

        public override async void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
        {
            base.OnNavigatedTo(e, viewModelState);
            await LoadDataAsync();
        }

        public async Task LoadDataAsync()
        {
            ExamleLogics.Clear();

            var data = await _exampleLogicService.GetExampleLogicAsync(_wheel, _lidarDistance, _ultrasonic);

            foreach (var item in data)
            {
                ExamleLogics.Add(item);
            }
        }

        public void SetDefaultSelection()
        {
            Selected = ExamleLogics.FirstOrDefault();
        }
    }
}
