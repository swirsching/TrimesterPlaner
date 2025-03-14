using System.Net;
using System.Net.Http;
using System.Windows;
using System.Windows.Input;
using TrimesterPlaner.Extensions;
using TrimesterPlaner.Models;
using TrimesterPlaner.Providers;
using TrimesterPlaner.Utilities;

namespace TrimesterPlaner.ViewModels
{
    public enum TicketSortingMode
    {
        Alphabetically,
        ByRank,
        BySize,
        ByUnplannedPT,
    }

    public class TicketProviderViewModel : BindableBase
    {
        public TicketProviderViewModel(ITicketProvider ticketProvider) : base()
        {
            TicketProvider = ticketProvider;
            Tickets = ticketProvider.GetAll();
            ReloadTicketsCommand = new RelayCommand((o) => ReloadTickets());
            SortTicketsCommand = new RelayCommand((o) => SortTickets((TicketSortingMode)o!));
        }

        private ITicketProvider TicketProvider { get; }

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
            var tickets = Tickets;
            Tickets = [];

            try
            {
                Tickets = await TicketProvider.ReloadTicketsAsync();
            } 
            catch (HttpRequestException e)
            {
                if (e.StatusCode == HttpStatusCode.BadRequest)
                {
                    MessageBox.Show("Der Trimester Planer konnte keine Tickets laden. Bitte überprüfe deine JQL.", "JQL invalid");
                }
                else
                {
                    MessageBox.Show("Der Trimester Planer konnte sich nicht mit Jira verbinden. Bitte überprüfe deine Internetverbindung.", "Keine Internetverbindung");   
                }
                Tickets = tickets;
            }
        }

        private void SortTickets(TicketSortingMode sortingMode)
        {
            Tickets = [];
            Tickets = TicketProvider.SortTickets(sortingMode switch
            {
                TicketSortingMode.Alphabetically => SortTicketsAlphabetically,
                TicketSortingMode.ByRank => SortTicketsByRank,
                TicketSortingMode.BySize => SortTicketsBySize,
                TicketSortingMode.ByUnplannedPT => SortTicketsByUnplannedPT,
                _ => throw new NotImplementedException(),
            });
        }

        private static int SortTicketsAlphabetically(Ticket a, Ticket b)
        {
            return a.Summary.CompareTo(b.Summary);
        }

        private static int SortTicketsByRank(Ticket a, Ticket b)
        {
            return a.Rank.CompareTo(b.Rank);
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