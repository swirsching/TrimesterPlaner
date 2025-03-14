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

    public class MainWindowViewModel 
        : BindableBase
        , IEntwicklungsplanManager
        , IConfigManager
        , IDeveloperManager
        , IDeveloperProvider
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

        public void Load(Config? config)
        {
            if (config is null)
            {
                return;
            }

            if (config.Settings is not null)
            {
                InjectExtension.ServiceProvider!.GetRequiredService<ISettingsProvider>().Set(config.Settings);
            }
            Developers.ClearAndAdd(config.Developers, new((a, b) => a.Abbreviation.CompareTo(b.Abbreviation)));
            InjectExtension.ServiceProvider!.GetRequiredService<IVacationProvider>().SetAll(config.Vacations);
            InjectExtension.ServiceProvider!.GetRequiredService<ITicketProvider>().SetAll(config.Tickets);            
            InjectExtension.ServiceProvider!.GetRequiredService<IPlanProvider>().SetAll(config.Plans);

            IsDirty = true;
        }

        public Config Save() => new()
        {
            Settings = InjectExtension.ServiceProvider!.GetRequiredService<ISettingsProvider>().Get(),
            Developers = [.. Developers],
            Vacations = [.. InjectExtension.ServiceProvider!.GetRequiredService<IVacationProvider>().GetAll()],
            Tickets = [.. InjectExtension.ServiceProvider!.GetRequiredService<ITicketProvider>().GetAll()],
            Plans = [.. InjectExtension.ServiceProvider!.GetRequiredService<IPlanProvider>().GetAll()],
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

        public IEnumerable<Developer> GetDevelopers()
        {
            return Developers;
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
            InjectExtension.ServiceProvider!.GetRequiredService<IPlanProvider>().RemovePlans(developer);

            Developers.Remove(developer);
            IsDirty = true;
        }

        private ObservableCollection<Developer> Developers { get; } = [];

        private bool IsDirty { get; set; } = true;
        private PreparedData? LastPreparedData { get; set; }
        private SvgDocument? LastResult { get; set; }
    }
}