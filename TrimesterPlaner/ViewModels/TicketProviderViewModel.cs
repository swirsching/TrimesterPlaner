using System.Windows.Input;
using TrimesterPlaner.Extensions;
using TrimesterPlaner.Models;
using TrimesterPlaner.Utilities;

namespace TrimesterPlaner.ViewModels
{
    public interface ITicketProvider
    {
        public IEnumerable<Ticket> GetTickets();
    }

    public enum TicketSortingMode
    {
        Alphabetically,
        BySize,
        ByUnplannedPT,
    }

    public class TicketProviderViewModel : BindableBase
    {
        public TicketProviderViewModel(ITicketProvider ticketProvider, ITicketManager ticketManager) : base()
        {
            Tickets = ticketProvider.GetTickets();
            TicketManager = ticketManager;
            ReloadTicketsCommand = new RelayCommand((o) => ReloadTickets());
            SortTicketsCommand = new RelayCommand((o) => SortTickets((TicketSortingMode)o!));
        }

        private ITicketManager TicketManager { get; }

        private IEnumerable<Ticket> _Tickets = [];
        public IEnumerable<Ticket> Tickets
        {
            get => _Tickets; 
            set => SetProperty(ref _Tickets, value);
        }

        public ICommand ReloadTicketsCommand { get; }
        public ICommand SortTicketsCommand { get; }

        private async void ReloadTickets()
        {
            Tickets = [];
            Tickets = await TicketManager.ReloadTicketsAsync();
        }

        private void SortTickets(TicketSortingMode sortingMode)
        {
            Tickets = [];
            Tickets = TicketManager.SortTickets(sortingMode switch
            {
                TicketSortingMode.Alphabetically => SortTicketsAlphabetically,
                TicketSortingMode.BySize => SortTicketsBySize,
                TicketSortingMode.ByUnplannedPT => SortTicketsByUnplannedPT,
                _ => throw new NotImplementedException(),
            });
        }

        private static int SortTicketsAlphabetically(Ticket a, Ticket b)
        {
            return a.Summary.CompareTo(b.Summary);
        }

        private static int SortTicketsBySize(Ticket a, Ticket b)
        {
            return b.GetTotalPT().CompareTo(a.GetTotalPT());
        }

        private static int SortTicketsByUnplannedPT(Ticket a, Ticket b)
        {
            return (b.GetTotalPT() - b.GetPlannedPT()).CompareTo(a.GetTotalPT() - a.GetPlannedPT());
        }
    }
}