using System;

using Application.Core.Models;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using ExampleLogic;

namespace Application.Views
{
    public sealed partial class ExampleLogicDetailControl : UserControl
    {
        public IExampleLogic MasterMenuItem
        {
            get { return GetValue(MasterMenuItemProperty) as IExampleLogic; }
            set { SetValue(MasterMenuItemProperty, value); }
        }

        public static readonly DependencyProperty MasterMenuItemProperty = DependencyProperty.Register("MasterMenuItem", typeof(IExampleLogic), typeof(ExampleLogicDetailControl), new PropertyMetadata(null, OnMasterMenuItemPropertyChanged));

        public ExampleLogicDetailControl()
        {
            InitializeComponent();
        }

        private static void OnMasterMenuItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ExampleLogicDetailControl;
            control.ForegroundElement.ChangeView(0, 0, 1);
        }
    }
}
