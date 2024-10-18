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
                _ => throw new NotImplementedException(),
            });
        }

        private static int SortTicketsAlphabetically(Ticket a, Ticket b)
        {
            if (TrySortingByPromised(a, b, out int value))
            {
                return value;
            }

            return a.Summary.CompareTo(b.Summary);
        }

        private static int SortTicketsBySize(Ticket a, Ticket b)
        {
            if (TrySortingByPromised(a, b, out int value))
            {
                return value;
            }

            return b.GetTotalPT().CompareTo(a.GetTotalPT());
        }

        private static bool TrySortingByPromised(Ticket a, Ticket b, out int value)
        {
            if (a.Promised != b.Promised)
            {
                value = (b.Promised ? 1 : 0) - (a.Promised ? 1 : 0);
                return true;
            }

            value = 0;
            return false;
        }
    }
}