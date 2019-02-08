using System;

using Application.ViewModels;

using Windows.UI.Xaml.Controls;

namespace Application.Views
{
    public sealed partial class WheelsPage : Page
    {
        private WheelsViewModel ViewModel => DataContext as WheelsViewModel;

        public WheelsPage()
        {
            InitializeComponent();
        }
    }
}
