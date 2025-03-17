using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TrimesterPlaner.Extensions;
using TrimesterPlaner.Models;
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
            var container = fe.FindAncestor<ItemsControl>();
            var tickets = (ObservableCollection<Ticket>)((TicketProviderViewModel)container!.DataContext).Tickets;
            var ticket = ((TicketViewModel)fe.DataContext).Ticket;

            tickets.Remove(ticket);
            DragDrop.DoDragDrop(container, new DataObject(ticket), DragDropEffects.Move);
        }

        private void Ticket_Drop(object sender, DragEventArgs e)
        {
            var fe = (FrameworkElement)sender;
            var container = fe.FindAncestor<ItemsControl>();
            var tickets = (ObservableCollection<Ticket>)((TicketProviderViewModel)container!.DataContext).Tickets;
            var ticket = ((TicketViewModel)fe.DataContext).Ticket;

            tickets.Insert(tickets.IndexOf(ticket), (Ticket)e.Data.GetData(typeof(Ticket)));
        }
    }
}