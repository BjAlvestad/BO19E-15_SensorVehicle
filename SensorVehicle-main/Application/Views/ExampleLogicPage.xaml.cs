using System;

using Application.ViewModels;

using Microsoft.Toolkit.Uwp.UI.Controls;

using Windows.UI.Xaml.Controls;

namespace Application.Views
{
    public sealed partial class ExampleLogicPage : Page
    {
        private ExampleLogicViewModel ViewModel => DataContext as ExampleLogicViewModel;

        public ExampleLogicPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            // Workaround for issue on MasterDetail Control. Find More info at https://github.com/Microsoft/WindowsTemplateStudio/issues/2739.
            if (MasterDetailsViewControl.ViewState == MasterDetailsViewState.Both)
            {
                ViewModel.SetDefaultSelection();
            }
        }
    }
}
