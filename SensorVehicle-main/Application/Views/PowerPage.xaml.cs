using System;

using Application.ViewModels;

using Windows.UI.Xaml.Controls;

namespace Application.Views
{
    public sealed partial class PowerPage : Page
    {
        private PowerViewModel ViewModel => DataContext as PowerViewModel;

        public PowerPage()
        {
            InitializeComponent();
        }
    }
}
