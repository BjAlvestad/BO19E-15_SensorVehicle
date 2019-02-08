using System;

using Application.ViewModels;

using Windows.UI.Xaml.Controls;

namespace Application.Views
{
    public sealed partial class UltrasonicPage : Page
    {
        private UltrasonicViewModel ViewModel => DataContext as UltrasonicViewModel;

        public UltrasonicPage()
        {
            InitializeComponent();
        }
    }
}
