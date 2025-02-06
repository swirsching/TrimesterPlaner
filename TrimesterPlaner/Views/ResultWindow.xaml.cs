using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TrimesterPlaner.Views
{
    public partial class ResultWindow : Window
    {
        public ResultWindow()
        {
            InitializeComponent();
        }

        private void Image_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                ((Image)sender).Height += 0.2 * e.Delta;
            }
        }
    }
}