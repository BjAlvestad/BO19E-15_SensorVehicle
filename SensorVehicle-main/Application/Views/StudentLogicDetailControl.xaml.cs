using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using StudentLogic;

namespace Application.Views
{
    public sealed partial class StudentLogicDetailControl : UserControl
    {
        public StudentLogicBase MasterMenuItem
        {
            get { return GetValue(MasterMenuItemProperty) as StudentLogicBase; }
            set { SetValue(MasterMenuItemProperty, value); }
        }

        public static readonly DependencyProperty MasterMenuItemProperty = DependencyProperty.Register("MasterMenuItem", typeof(StudentLogicBase), typeof(StudentLogicDetailControl), new PropertyMetadata(null, OnMasterMenuItemPropertyChanged));

        public StudentLogicDetailControl()
        {
            InitializeComponent();
        }

        private static void OnMasterMenuItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as StudentLogicDetailControl;
            control.ForegroundElement.ChangeView(0, 0, 1);
        }
    }
}
