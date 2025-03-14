﻿using System.Collections.ObjectModel;
using TrimesterPlaner.Extensions;
using TrimesterPlaner.Models;
using TrimesterPlaner.ViewModels;

namespace TrimesterPlaner.Providers
{
    public interface ITicketProvider : IProvider<Ticket>
    {
        public Task<IEnumerable<Ticket>> ReloadTicketsAsync();
        public IEnumerable<Ticket> SortTickets(Comparison<Ticket> comparison);
    }

    public class TicketProvider(
        IEntwicklungsplanManager entwicklungsplanManager, 
        IPlanProvider planProvider,
        ISettingsProvider settingsProvider,
        JiraClient jiraClient)
        : ITicketProvider
    {
        private ObservableCollection<Ticket> Tickets { get; } = [];

        public IEnumerable<Ticket> GetAll()
        {
            return Tickets;
        }

        public void SetAll(IEnumerable<Ticket> values)
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
                planProvider.RemovePlans(ticket);
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
            var loadedTickets = await jiraClient.LoadTickets(settingsProvider.Get().JQL, true);
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

            entwicklungsplanManager.RefreshEntwicklungsplan();
            return Tickets;
        }

        public IEnumerable<Ticket> SortTickets(Comparison<Ticket> comparison)
        {
            Tickets.ClearAndAdd([.. Tickets], comparison);
            return Tickets;
        }
    }
}