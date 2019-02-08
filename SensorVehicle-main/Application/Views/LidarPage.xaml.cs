using System;

using Application.ViewModels;

using Windows.UI.Xaml.Controls;

namespace Application.Views
{
    public sealed partial class LidarPage : Page
    {
        private LidarViewModel ViewModel => DataContext as LidarViewModel;

        public LidarPage()
        {
            InitializeComponent();
        }
    }
}
