using System;

using Application.ViewModels;

using Windows.UI.Xaml.Controls;

namespace Application.Views
{
    public sealed partial class EquipmentOverviewPage : Page
    {
        private EquipmentOverviewViewModel ViewModel => DataContext as EquipmentOverviewViewModel;

        public EquipmentOverviewPage()
        {
            InitializeComponent();
        }
    }
}
