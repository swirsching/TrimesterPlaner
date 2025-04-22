using Statistics.Models;
using Statistics.Providers;
using System.Windows.Input;
using Utilities.Extensions;
using Utilities.Utilities;

namespace Statistics.ViewModels
{
    public class StatisticProviderViewModel : PropertyChangedBase
    {
        public IEnumerable<Statistic> Statistics { get; } = Inject.Require<IStatisticsProvider>().Get();

        public ICommand AddStatisticCommand { get; } = new RelayCommand((o) => Inject.Require<IStatisticsProvider>().AddStatistic());
    }
}