using System.Windows;
using System.Windows.Controls;

namespace TrimesterPlaner.Controls
{
    public partial class DragAdorner : UserControl
    {
        public DragAdorner()
        {
            InitializeComponent();
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(DragAdorner), new PropertyMetadata(string.Empty));
    }
}