using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;
using TrimesterPlaner.Extensions;
using TrimesterPlaner.Services;
using TrimesterPlaner.ViewModels;

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
            var resultWindow = InjectExtension.ServiceProvider!.GetRequiredService<ResultWindow>();
            resultWindow.Owner = this.FindAncestor<Window>();
            resultWindow.Show();
            resultWindow.Closed += (sender, e) => IsShowingResultWindow = false;
            IsShowingResultWindow = true;
            InjectExtension.ServiceProvider!.GetRequiredService<IPlaner>().RefreshPlan();
        }

        public bool IsShowingResultWindow
        {
            get => (bool)GetValue(IsShowingResultWindowProperty);
            set => SetValue(IsShowingResultWindowProperty, value);
        }
        public static readonly DependencyProperty IsShowingResultWindowProperty = DependencyProperty.Register("IsShowingResultWindow", typeof(bool), typeof(MainWindowMenuView), new PropertyMetadata(false));
    }
}