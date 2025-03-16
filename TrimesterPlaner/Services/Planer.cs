using Svg;
using System.Windows.Threading;
using TrimesterPlaner.Providers;

namespace TrimesterPlaner.Services
{
    public delegate void PlanChangedHandler(PreparedData? data, SvgDocument? result);
    public interface IPlaner
    {
        public event PlanChangedHandler? PlanChanged;
        public void RefreshPlan();
        public PreparedData? GetLastPreparedData();
        public SvgDocument? GetLastPlan();
    }

    public class Planer : IPlaner
    {
        public Planer(IConfigProvider configProvider, IGenerator generator, IPreparator preparator)
        {
            ConfigProvider = configProvider;
            Generator = generator;
            Preparator = preparator;

            var timer = new DispatcherTimer(TimeSpan.FromMilliseconds(100), DispatcherPriority.Background, GenerateIfDirty, Dispatcher.CurrentDispatcher);
        }

        private IConfigProvider ConfigProvider { get; }
        private IGenerator Generator { get; }
        private IPreparator Preparator { get; }

        public event PlanChangedHandler? PlanChanged;
        private void GenerateIfDirty(object? sender, EventArgs e)
        {
            if (IsDirty)
            {
                LastPreparedData = Preparator.Prepare(ConfigProvider.Get());
                LastResult = LastPreparedData is null ? null : Generator.Generate(LastPreparedData);
                PlanChanged?.Invoke(LastPreparedData, LastResult);
                IsDirty = false;
            }
        }

        public void RefreshPlan()
        {
            IsDirty = true;
        }

        public PreparedData? GetLastPreparedData()
        {
            return LastPreparedData;
        }

        public SvgDocument? GetLastPlan()
        {
            return LastResult;
        }

        private bool IsDirty { get; set; } = true;
        private PreparedData? LastPreparedData { get; set; }
        private SvgDocument? LastResult { get; set; }
    }
}
