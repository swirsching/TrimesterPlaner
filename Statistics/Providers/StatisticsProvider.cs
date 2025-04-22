using Statistics.Models;
using System.Collections.ObjectModel;
using Utilities.Extensions;
using Utilities.Providers;

namespace Statistics.Providers
{
    public interface IStatisticsProvider : ICollectionProvider<Statistic>
    {
        public void AddStatistic();
    }

    public class StatisticsProvider : IStatisticsProvider
    {
        private ObservableCollection<Statistic> Statistics { get; } = [];

        public IEnumerable<Statistic> Get()
        {
            return Statistics;
        }

        public void Set(IEnumerable<Statistic> values)
        {
            Statistics.ClearAndAdd(values);
        }

        public void Remove(Statistic value)
        {
            Statistics.Remove(value);
        }

        public void AddStatistic()
        {
            Statistics.Add(new DateBasedStatistic());
        }
    }
}