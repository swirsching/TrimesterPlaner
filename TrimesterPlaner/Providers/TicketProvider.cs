using System.Collections.ObjectModel;
using TrimesterPlaner.Extensions;
using TrimesterPlaner.Models;
using TrimesterPlaner.Services;

namespace TrimesterPlaner.Providers
{
    public interface ITicketProvider : ICollectionProvider<Ticket>
    {
        public Task<IEnumerable<Ticket>> ReloadTicketsAsync();
        public IEnumerable<Ticket> SortTickets(Comparison<Ticket> comparison);
    }

    public class TicketProvider : ITicketProvider
    {
        private ObservableCollection<Ticket> Tickets { get; } = [];

        public IEnumerable<Ticket> Get()
        {
            return Tickets;
        }

        public void Set(IEnumerable<Ticket> values)
        {
            Tickets.ClearAndAdd(values);
        }

        public void Remove(Ticket value)
        {
            RemoveTicket(value, true);
        }

        private bool RemoveTicket(Ticket ticket, bool force)
        {
            if (force)
            {
                Inject.Require<IPlanProvider>().RemovePlans(ticket);
            }
            if (ticket.Plans.Count == 0)
            {
                Tickets.Remove(ticket);
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<Ticket>> ReloadTicketsAsync()
        {
            string jql = Inject.Require<ISettingsProvider>().Get().JQL;
            var jiraClient = Inject.Require<IJiraClient>();
            var loadedTickets = await jiraClient.LoadTickets(jql, true);
            if (loadedTickets is null)
            {
                return [];
            }

            TicketByKeyComparer comparer = new();

            if (Tickets.Count > 0)
            {
                var additionalKeys = from ticket in Tickets
                                     where !loadedTickets.Contains(ticket, comparer)
                                     select ticket.Key;
                if (additionalKeys.Any())
                {
                    string additionalJQL = $"key in ({string.Join(",", additionalKeys)})";
                    var additionalTickets = await jiraClient.LoadTickets(additionalJQL, false);
                    if (additionalTickets is not null)
                    {
                        loadedTickets = loadedTickets.Concat(additionalTickets);
                    }
                }
            }

            foreach (Ticket loadedTicket in loadedTickets)
            {
                if (Tickets.Contains(loadedTicket, comparer))
                {
                    var ticketToUpdate = Tickets.Single((ticket) => comparer.Equals(loadedTicket, ticket));
                    ticketToUpdate.Update(loadedTicket);
                }
                else
                {
                    Tickets.Add(loadedTicket);
                }
            }

            Inject.Require<IPlaner>().RefreshPlan();
            return Tickets;
        }

        public IEnumerable<Ticket> SortTickets(Comparison<Ticket> comparison)
        {
            Tickets.ClearAndAdd([.. Tickets], comparison);
            return Tickets;
        }
    }
}