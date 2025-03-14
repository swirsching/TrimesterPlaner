using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TrimesterPlaner.Extensions;
using TrimesterPlaner.ViewModels;

namespace TrimesterPlaner.Views
{
    public partial class TicketView : UserControl
    {
        public TicketView()
        {
            InitializeComponent();
        }

        private void Ticket_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var fe = (FrameworkElement)sender;
            var vm = (TicketViewModel)fe.DataContext;
            var data = new DataObject(vm.Ticket);

            var container = fe.FindAncestor<ItemsControl>();
            DragDrop.DoDragDrop(container, data, DragDropEffects.Move);
        }
    }
}