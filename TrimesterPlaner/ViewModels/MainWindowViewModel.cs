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

    public class MainWindowViewModel 
        : BindableBase
        , IEntwicklungsplanManager
    {
        public MainWindowViewModel(
            IConfigProvider configProvider,
            IGenerator generator, 
            IPreparator preparator)
        {
            ConfigProvider = configProvider;
            Generator = generator;
            Preparator = preparator;

            var timer = new DispatcherTimer(TimeSpan.FromMilliseconds(100), DispatcherPriority.Background, GenerateIfDirty, Dispatcher.CurrentDispatcher);
        }

        private IConfigProvider ConfigProvider { get; }
        private IGenerator Generator { get; }
        private IPreparator Preparator { get; }

        public event EntwicklungsplanChangedHandler? EntwicklungsplanChanged;
        private void GenerateIfDirty(object? sender, EventArgs e)
        {
            if (IsDirty)
            {
                LastPreparedData = Preparator.Prepare(ConfigProvider.Get());
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