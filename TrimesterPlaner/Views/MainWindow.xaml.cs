using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TrimesterPlaner.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Image_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            ((Image)sender).Height += 0.2 * e.Delta;
        }
    }
}