using System;

using Application.ViewModels;

using Windows.UI.Xaml.Controls;

namespace Application.Views
{
    public sealed partial class InfoPage : Page
    {
        private InfoViewModel ViewModel => DataContext as InfoViewModel;

        public InfoPage()
        {
            InitializeComponent();
        }
    }
}
