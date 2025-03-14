using Microsoft.Extensions.DependencyInjection;
using Svg;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using TrimesterPlaner.Extensions;
using TrimesterPlaner.Models;
using TrimesterPlaner.Providers;
using TrimesterPlaner.Utilities;

namespace TrimesterPlaner.ViewModels
{
    public delegate void EntwicklungsplanChangedHandler(PreparedData? data, SvgDocument? result);
    public interface IEntwicklungsplanManager
    {
        public event EntwicklungsplanChangedHandler? EntwicklungsplanChanged;
        public void RefreshEntwicklungsplan();
        public PreparedData? GetLastPreparedData();
        public SvgDocument? GetLastResult();
        public Settings GetSettings();
    }

    public interface IConfigManager
    {
        public void Load(Config? config);
        public Config Save();
    }

    public interface IDeveloperManager
    {
        public Developer AddDeveloper(string name);
        public void RemoveDeveloper(Developer developer);
    }

    public interface ITicketManager
    {
        public Task<IEnumerable<Ticket>> ReloadTicketsAsync();
        public IEnumerable<Ticket> SortTickets(Comparison<Ticket> comparison);
        public void RemoveTicket(Ticket ticket);
    }

    public interface IPlanManager
    {
        public void AddTicketPlan(Developer developer, Ticket ticket);
        public void AddBugPlan(Developer developer);
        public void AddSpecialPlan(Developer developer);
        public void RemovePlan(Plan plan);
        public void MoveUp(Plan plan);
        public void MoveDown(Plan plan);
    }

    public class MainWindowViewModel 
        : BindableBase
        , IEntwicklungsplanManager
        , IConfigManager
        , IDeveloperManager
        , ITicketManager
        , IPlanManager
        , IDeveloperProvider
        , ITicketProvider
        , IPlanProvider
    {
        public MainWindowViewModel(
            JiraClient jiraClient, 
            IGenerator generator, 
            IPreparator preparator)
        {
            JiraClient = jiraClient;
            Generator = generator;
            Preparator = preparator;

            var timer = new DispatcherTimer(TimeSpan.FromMilliseconds(100), DispatcherPriority.Background, GenerateIfDirty, Dispatcher.CurrentDispatcher);
        }

        private JiraClient JiraClient { get; }
        private IGenerator Generator { get; }
        private IPreparator Preparator { get; }

        public void RemoveTicket(Ticket ticket)
        {
            RemoveTicket(ticket, true);
        }

        private bool RemoveTicket(Ticket ticket, bool force)
        {
            if (force)
            {
                List<Plan> plansToRemove = [.. ticket.Plans];
                foreach (Plan plan in plansToRemove)
                {
                    RemovePlan(plan);
                }
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
            TicketByKeyComparer comparer = new();

            var loadedTickets = await JiraClient.LoadTickets(Settings.JQL, true);
            if (loadedTickets is null)
            {
                return [];
            }

            if (Tickets.Count > 0)
            {
                var additionalKeys = from ticket in Tickets
                                     where !loadedTickets.Contains(ticket, comparer)
                                     select ticket.Key;
                if (additionalKeys.Any())
                {
                    string additionalJQL = $"key in ({string.Join(",", additionalKeys)})";
                    var additionalTickets = await JiraClient.LoadTickets(additionalJQL, false);
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

            IsDirty = true;
            return Tickets;
        }

        public IEnumerable<Ticket> SortTickets(Comparison<Ticket> comparison)
        {
            Tickets.ClearAndAdd([.. Tickets], comparison);
            return Tickets;
        }

        public void Load(Config? config)
        {
            if (config is null)
            {
                return;
            }

            if (config.Settings is not null)
            {
                Settings = config.Settings;
            }
            Developers.ClearAndAdd(config.Developers, new((a, b) => a.Abbreviation.CompareTo(b.Abbreviation)));
            InjectExtension.ServiceProvider!.GetRequiredService<IVacationProvider>().SetAll(config.Vacations);
            Tickets.ClearAndAdd(config.Tickets);
            Plans.ClearAndAdd(config.Plans);

            IsDirty = true;
        }

        public Config Save() => new()
        {
            Settings = Settings,
            Developers = [.. Developers],
            Vacations = [.. InjectExtension.ServiceProvider!.GetRequiredService<IVacationProvider>().GetAll()],
            Tickets = [.. Tickets],
            Plans = [.. Plans],
        };

        public event EntwicklungsplanChangedHandler? EntwicklungsplanChanged;
        private void GenerateIfDirty(object? sender, EventArgs e)
        {
            if (IsDirty)
            {
                LastPreparedData = Preparator.Prepare(Save());
                LastResult = LastPreparedData is null ? null : Generator.Generate(LastPreparedData);
                EntwicklungsplanChanged?.Invoke(LastPreparedData, LastResult);
                IsDirty = false;
            }
        }

        public void RefreshEntwicklungsplan()
        {
            IsDirty = true;
        }

        public PreparedData? GetLastPreparedData()
        {
            return LastPreparedData;
        }

        public SvgDocument? GetLastResult()
        {
            return LastResult;
        }

        public Settings GetSettings()
        {
            return Settings;
        }

        public IEnumerable<Developer> GetDevelopers()
        {
            return Developers;
        }

        public IEnumerable<Ticket> GetTickets()
        {
            return Tickets;
        }

        public IEnumerable<Plan> GetPlans()
        {
            return Plans;
        }

        public Developer AddDeveloper(string name)
        {
            Developer developer = new()
            {
                Name = name,
                Abbreviation = name[0..3].ToUpper(),
            };
            Developers.Add(developer);
            IsDirty = true;
            return developer;
        }

        public void RemoveDeveloper(Developer developer)
        {
            InjectExtension.ServiceProvider!.GetRequiredService<IVacationProvider>().RemoveVacations(developer);
            List<Plan> plansToRemove = [.. developer.Plans];
            foreach (var plan in plansToRemove)
            {
                RemovePlan(plan); 
            }

            Developers.Remove(developer);
            IsDirty = true;
        }

        private void AddPlan(Plan plan)
        {
            Plans.Add(plan);
            IsDirty = true;
        }

        public void AddTicketPlan(Developer developer, Ticket ticket) => AddPlan(new TicketPlan() { Developer = developer, Ticket = ticket });
        public void AddBugPlan(Developer developer) => AddPlan(new BugPlan() { Developer = developer });
        public void AddSpecialPlan(Developer developer) => AddPlan(new SpecialPlan() { Developer = developer });

        public void RemovePlan(Plan plan)
        {
            plan.Developer = null;
            if (plan is TicketPlan ticketPlan)
            {
                ticketPlan.Ticket = null;
            }
            Plans.Remove(plan);
            IsDirty = true;
        }

        private void ReorderPlansForDeveloper(Developer developer)
        { 
            developer.Plans.Clear();
            foreach (Plan plan in Plans)
            {
                if (plan.Developer == developer)
                {
                    developer.Plans.Add(plan);
                }
            }
        }

        public void MoveUp(Plan plan)
        {
            int currentIdx = Plans.IndexOf(plan);
            for (int idx = currentIdx - 1; idx >= 0; idx--)
            {
                if (plan.Developer == Plans[idx].Developer)
                {
                    Plans.Move(currentIdx, idx);
                    ReorderPlansForDeveloper(plan.Developer!);
                    IsDirty = true;
                    return;
                }
            }
        }

        public void MoveDown(Plan plan)
        {
            int currentIdx = Plans.IndexOf(plan);
            for (int idx = currentIdx + 1; idx < Plans.Count; idx++)
            {
                if (plan.Developer == Plans[idx].Developer)
                {
                    Plans.Move(currentIdx, idx);
                    ReorderPlansForDeveloper(plan.Developer!);
                    IsDirty = true;
                    return;
                }
            }
        }

        private Settings _Settings = new();
        public Settings Settings
        {
            get => _Settings;
            set => SetProperty(ref _Settings, value);
        }

        private ObservableCollection<Developer> Developers { get; } = [];
        private ObservableCollection<Ticket> Tickets { get; } = [];
        private ObservableCollection<Plan> Plans { get; } = [];

        private bool IsDirty { get; set; } = true;
        private PreparedData? LastPreparedData { get; set; }
        private SvgDocument? LastResult { get; set; }
    }
}