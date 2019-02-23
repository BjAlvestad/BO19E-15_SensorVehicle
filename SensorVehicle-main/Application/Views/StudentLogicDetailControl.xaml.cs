﻿using System;

using Application.Core.Models;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Application.Views
{
    public sealed partial class StudentLogicDetailControl : UserControl
    {
        public SampleOrder MasterMenuItem
        {
            get { return GetValue(MasterMenuItemProperty) as SampleOrder; }
            set { SetValue(MasterMenuItemProperty, value); }
        }

        public static readonly DependencyProperty MasterMenuItemProperty = DependencyProperty.Register("MasterMenuItem", typeof(SampleOrder), typeof(StudentLogicDetailControl), new PropertyMetadata(null, OnMasterMenuItemPropertyChanged));

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