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

namespace Application.ViewModels
{
    public class StudentLogicViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly ISampleDataService _sampleDataService;

        public StudentLogicViewModel(INavigationService navigationServiceInstance, ISampleDataService sampleDataServiceInstance)
        {
            _navigationService = navigationServiceInstance;
            _sampleDataService = sampleDataServiceInstance;
        }

        private SampleOrder _selected;

        public SampleOrder Selected
        {
            get => _selected;
            set => SetProperty(ref _selected, value);
        }

        public ObservableCollection<SampleOrder> SampleItems { get; } = new ObservableCollection<SampleOrder>();

        public override async void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
        {
            base.OnNavigatedTo(e, viewModelState);
            await LoadDataAsync();
        }

        public async Task LoadDataAsync()
        {
            SampleItems.Clear();

            var data = await _sampleDataService.GetSampleModelDataAsync();

            foreach (var item in data)
            {
                SampleItems.Add(item);
            }
        }

        public void SetDefaultSelection()
        {
            Selected = SampleItems.FirstOrDefault();
        }
    }
}
