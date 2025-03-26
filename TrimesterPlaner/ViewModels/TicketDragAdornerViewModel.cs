using TrimesterPlaner.Models;
using TrimesterPlaner.Utilities;

namespace TrimesterPlaner.ViewModels
{
    public class TicketDragAdornerViewModel(Ticket ticket) : BindableBase
    {
        public string Key { get => ticket.Key; }
        public string Summary { get => ticket.Summary; }
    }
}