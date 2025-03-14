using System.Windows;
using System.Windows.Controls;
using TrimesterPlaner.Models;
using TrimesterPlaner.ViewModels;

namespace TrimesterPlaner.Views
{
    public partial class TicketsView : UserControl
    {
        public TicketsView()
        {
            InitializeComponent();
        }

        private void Tickets_Drop(object sender, DragEventArgs e)
        {
            var vm = (TicketProviderViewModel)DataContext;
            var ticket = e.Data.GetData(typeof(Ticket));
        }
    }
}