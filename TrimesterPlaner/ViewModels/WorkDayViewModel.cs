using TrimesterPlaner.Models;
using TrimesterPlaner.Services;

namespace TrimesterPlaner.ViewModels
{
    public class WorkDayViewModel(
        DayOfWeek dayOfWeek,
        Developer developer,
        IPlaner trimesterPlaner) : BaseViewModel(trimesterPlaner)
    {
        public DayOfWeek DayOfWeek { get; } = dayOfWeek;

        public bool IsWorkDay
        {
            get => !developer.FreeDays.Contains(DayOfWeek);
            set
            {
                if (value)
                {
                    developer.FreeDays.Remove(DayOfWeek);
                }
                else
                {
                    developer.FreeDays.Add(DayOfWeek);
                }
                OnPropertyChanged();
            }
        }
    }
}