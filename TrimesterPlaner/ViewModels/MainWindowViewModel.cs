﻿using Microsoft.Win32;
using Svg;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Threading;
using TextCopy;
using TrimesterPlaner.Extensions;
using TrimesterPlaner.Models;
using TrimesterPlaner.Utilities;

namespace TrimesterPlaner.ViewModels
{
    public delegate void EntwicklungsplanChangedHandler(PreparedData? data);
    public interface IEntwicklungsplanManager
    {
        public event EntwicklungsplanChangedHandler? EntwicklungsplanChanged;
        public void RefreshEntwicklungsplan();
        public PreparedData? GetLastPreparedData();
    }

    public interface IDeveloperManager
    {
        public Developer AddDeveloper(string name);
        public void RemoveDeveloper(Developer developer);
    }

    public interface IVacationManager
    {
        public void AddVacation(Developer developer);
        public void RemoveVacation(Vacation vacation);
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
        , IDeveloperManager
        , IVacationManager
        , ITicketManager
        , IPlanManager
        , IDeveloperProvider
        , IVacationProvider
        , ITicketProvider
        , IPlanProvider
    {
        public MainWindowViewModel(
            ConfluenceClient confluenceClient, 
            JiraClient jiraClient,
            IGenerator generator,
            IPreparator preparator,
            IConfigService configService)
        {
            JiraClient = jiraClient;
            Generator = generator;
            Preparator = preparator;

            HasCAT = confluenceClient.HasCAT;

            LoadCommand = new RelayCommand((o) => Load(configService.LoadConfig(o as string)));
            SaveCommand = new RelayCommand((o) => configService.SaveConfig(MakeConfig()));
            SaveCopyCommand = new RelayCommand((o) => configService.SaveConfigCopy(MakeConfig()));
            ExportCommand = new RelayCommand((o) => Export());
            CopyToClipboardCommand = new RelayCommand((o) => ClipboardService.SetText(Result.ConvertToPastableHTML()));
            PushToConfluenceCommand = new RelayCommand((o) => confluenceClient.UpdatePage(Result.ConvertToPastableHTML()));

            var timer = new DispatcherTimer(TimeSpan.FromMilliseconds(100), DispatcherPriority.Background, GenerateIfDirty, Dispatcher.CurrentDispatcher);
        }

        private JiraClient JiraClient { get; }
        private IGenerator Generator { get; }
        private IPreparator Preparator { get; }

        private void Export()
        {
            var dialog = new SaveFileDialog()
            {
                FileName = "TrimesterPlaner",
                DefaultExt = ".svg",
            };

            bool? ok = dialog.ShowDialog();
            if (ok == true)
            {
                Result?.Write(dialog.FileName);
            }
        }

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
                string additionalJQL = $"key in ({string.Join(",", additionalKeys)})";
                var additionalTickets = await JiraClient.LoadTickets(additionalJQL, false);
                if (additionalTickets is not null)
                {
                    loadedTickets = loadedTickets.Concat(additionalTickets);
                }
            }

            foreach (Ticket loadedTicket in loadedTickets)
            {
                if (Tickets.Contains(loadedTicket, comparer))
                {
                    var ticketToUpdate = Tickets.Single((ticket) => comparer.Equals(loadedTicket, ticket));
                    if (!ticketToUpdate.Summary.Equals(loadedTicket.Summary))
                    {
                        ticketToUpdate.Summary = loadedTicket.Summary;
                    }
                    if (!ticketToUpdate.Shirt.Equals(loadedTicket.Shirt))
                    {
                        ticketToUpdate.Shirt = loadedTicket.Shirt;
                    }
                    if (!ticketToUpdate.IsInJQL.Equals(loadedTicket.IsInJQL))
                    {
                        ticketToUpdate.IsInJQL = loadedTicket.IsInJQL;
                    }
                    if (ticketToUpdate.OriginalEstimate != loadedTicket.OriginalEstimate)
                    {
                        ticketToUpdate.OriginalEstimate = loadedTicket.OriginalEstimate;
                    }
                    if (ticketToUpdate.RemainingEstimate != loadedTicket.RemainingEstimate)
                    {
                        ticketToUpdate.RemainingEstimate = loadedTicket.RemainingEstimate;
                    }
                    if (ticketToUpdate.TimeSpent != loadedTicket.TimeSpent)
                    {
                        ticketToUpdate.TimeSpent = loadedTicket.TimeSpent;
                    }
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

        private void Load(Config? config)
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
            Vacations.ClearAndAdd(config.Vacations, new((a, b) =>
            {
                if (a.Start is null || b.Start is null || a.Start == b.Start)
                {
                    if (a.Developer is null || b.Developer is null)
                    {
                        return 0;
                    }

                    return a.Developer.Abbreviation.CompareTo(b.Developer.Abbreviation);
                }

                return a.Start < b.Start ? -1 : 1;
            }));
            Tickets.ClearAndAdd(config.Tickets);
            Plans.ClearAndAdd(config.Plans);

            IsDirty = true;
        }
        private Config MakeConfig() => new()
        {
            Settings = Settings,
            Developers = [.. Developers],
            Vacations = [.. Vacations],
            Tickets = [.. Tickets],
            Plans = [.. Plans],
        };

        public event EntwicklungsplanChangedHandler? EntwicklungsplanChanged;
        private void GenerateIfDirty(object? sender, EventArgs e)
        {
            if (IsDirty)
            {
                LastPreparedData = Preparator.Prepare(MakeConfig());
                Result = LastPreparedData is null ? null : Generator.Generate(LastPreparedData);
                EntwicklungsplanChanged?.Invoke(LastPreparedData);
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

        public Settings GetSettings()
        {
            return Settings;
        }

        public IEnumerable<Developer> GetDevelopers()
        {
            return Developers;
        }

        public IEnumerable<Vacation> GetVacations()
        {
            return Vacations;
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
            List<Vacation> vacationsToRemove = [.. developer.Vacations];
            List<Plan> plansToRemove = [.. developer.Plans];
            foreach (var vacation in vacationsToRemove)
            {
                RemoveVacation(vacation);
            }
            foreach (var plan in plansToRemove)
            {
                RemovePlan(plan); 
            }

            Developers.Remove(developer);
            IsDirty = true;
        }

        public void AddVacation(Developer developer)
        {
            Vacations.Add(new Vacation() { Developer = developer });
            IsDirty = true;
        }

        public void RemoveVacation(Vacation vacation)
        {
            vacation.Developer = null;
            Vacations.Remove(vacation);
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

        public bool HasCAT { get; }
        private ObservableCollection<Developer> Developers { get; } = [];
        private ObservableCollection<Vacation> Vacations { get; } = [];
        private ObservableCollection<Ticket> Tickets { get; } = [];
        private ObservableCollection<Plan> Plans { get; } = [];

        private bool _IsDirty = true;
        public bool IsDirty
        {
            get => _IsDirty;
            set => SetProperty(ref _IsDirty, value);
        }

        private PreparedData? LastPreparedData { get; set; }

        private SvgDocument? _Result = null;
        public SvgDocument? Result
        {
            get => _Result;
            set { if (SetProperty(ref _Result, value)) IsDirty = false; }
        }

        public ICommand LoadCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand SaveCopyCommand { get; }
        public ICommand ExportCommand { get; }
        public ICommand CopyToClipboardCommand { get; }
        public ICommand PushToConfluenceCommand { get; }
    }
}