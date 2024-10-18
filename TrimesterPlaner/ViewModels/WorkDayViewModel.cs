using TrimesterPlaner.Models;

namespace TrimesterPlaner.ViewModels
{
    public class WorkDayViewModel(DayOfWeek dayOfWeek, Developer developer, IEntwicklungsplanManager entwicklungsplanManager) : BaseViewModel(entwicklungsplanManager)
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