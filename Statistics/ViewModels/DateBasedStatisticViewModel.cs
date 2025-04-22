using Statistics.Models;
using Utilities.Utilities;

namespace Statistics.ViewModels
{
    public class DateBasedStatisticViewModel(DateBasedStatistic statistic) : PropertyChangedBase
    {
        public DateField? DateField
        {
            get => statistic.DateField;
            set
            {
                statistic.DateField = value;
                OnPropertyChanged();
            }
        }

        public Grouping? Grouping
        {
            get => statistic.Grouping;
            set
            {
                statistic.Grouping = value;
                OnPropertyChanged();
            }
        }
    }
}