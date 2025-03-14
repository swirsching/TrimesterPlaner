using Microsoft.Extensions.DependencyInjection;
using Svg;
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

    public class MainWindowViewModel 
        : BindableBase
        , IEntwicklungsplanManager
        , IConfigManager
    {
        public MainWindowViewModel(
            IGenerator generator, 
            IPreparator preparator)
        {
            Generator = generator;
            Preparator = preparator;

            var timer = new DispatcherTimer(TimeSpan.FromMilliseconds(100), DispatcherPriority.Background, GenerateIfDirty, Dispatcher.CurrentDispatcher);
        }

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
            InjectExtension.ServiceProvider!.GetRequiredService<IDeveloperProvider>().Set(config.Developers);
            InjectExtension.ServiceProvider!.GetRequiredService<IVacationProvider>().Set(config.Vacations);
            InjectExtension.ServiceProvider!.GetRequiredService<ITicketProvider>().Set(config.Tickets);            
            InjectExtension.ServiceProvider!.GetRequiredService<IPlanProvider>().Set(config.Plans);

            IsDirty = true;
        }

        public Config Save() => new()
        {
            Settings = InjectExtension.ServiceProvider!.GetRequiredService<ISettingsProvider>().Get(),
            Developers = [.. InjectExtension.ServiceProvider!.GetRequiredService<IDeveloperProvider>().Get()],
            Vacations = [.. InjectExtension.ServiceProvider!.GetRequiredService<IVacationProvider>().Get()],
            Tickets = [.. InjectExtension.ServiceProvider!.GetRequiredService<ITicketProvider>().Get()],
            Plans = [.. InjectExtension.ServiceProvider!.GetRequiredService<IPlanProvider>().Get()],
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

        private bool IsDirty { get; set; } = true;
        private PreparedData? LastPreparedData { get; set; }
        private SvgDocument? LastResult { get; set; }
    }
}