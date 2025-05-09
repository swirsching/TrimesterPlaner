﻿using System.Windows;
using System.Windows.Controls;
using TrimesterPlaner.Services;
using Utilities.Extensions;

namespace TrimesterPlaner.Views
{
    public partial class MainWindowMenuView : UserControl
    {
        public MainWindowMenuView()
        {
            InitializeComponent();
        }

        private void ShowResultWindow(object sender, RoutedEventArgs e)
        {
            var resultWindow = Inject.Require<ResultWindow>();
            resultWindow.Owner = this.FindAncestor<Window>();
            resultWindow.Show();
            resultWindow.Closed += (sender, e) => IsShowingResultWindow = false;
            IsShowingResultWindow = true;
            Inject.Require<IPlaner>().RefreshPlan();
        }

        public bool IsShowingResultWindow
        {
            get => (bool)GetValue(IsShowingResultWindowProperty);
            set => SetValue(IsShowingResultWindowProperty, value);
        }
        public static readonly DependencyProperty IsShowingResultWindowProperty = DependencyProperty.Register("IsShowingResultWindow", typeof(bool), typeof(MainWindowMenuView), new PropertyMetadata(false));
    }
}