using Statistics.Models;
using Statistics.Providers;
using System.Windows.Input;
using Utilities.Extensions;
using Utilities.Utilities;

namespace Statistics.ViewModels
{
    public class StatisticViewModel(Statistic statistic) : PropertyChangedBase
    {
        public ICommand RemoveCommand { get; } = new RelayCommand((o) => Inject.Require<IStatisticsProvider>().Remove(statistic));

        public Statistic Statistic { get => statistic; }

        public string Header
        {
            get => statistic.Header;
            set
            {
                statistic.Header = value;
                OnPropertyChanged();
            }
        }

        public string JQL
        {
            get => statistic.JQL;
            set
            {
                statistic.JQL = value;
                OnPropertyChanged();
            }
        }

        private bool _IsEditing = false;
        public bool IsEditing
        {
            get => _IsEditing;
            set => SetProperty(ref _IsEditing, value);
        }
    }
}