using TrimesterPlaner.Models;

namespace TrimesterPlaner.ViewModels
{
    public class DeveloperViewModel : BaseViewModel
    {
        public DeveloperViewModel(Developer developer, IEntwicklungsplanManager entwicklungsplanManager) : base(entwicklungsplanManager)
        {
            Developer = developer;
            WorkDays = from dayOfWeek in Enumerable.Range((int)DayOfWeek.Monday, 5)
                       select new WorkDayViewModel((DayOfWeek)(dayOfWeek % 7), Developer, entwicklungsplanManager);
        }

        private Developer Developer { get; }

        public string Abbreviation
        {
            get => Developer.Abbreviation;
            set
            {
                Developer.Abbreviation = value;
                OnPropertyChanged();
            }
        }

        public int FTE
        {
            get => Developer.FTE;
            set
            {
                Developer.FTE = value;
                OnPropertyChanged();
            }
        }

        public int Sonderrolle
        {
            get => Developer.Sonderrolle;
            set
            {
                Developer.Sonderrolle = value;
                OnPropertyChanged();
            }
        }

        public int Verwaltung
        {
            get => Developer.Verwaltung;
            set
            {
                Developer.Verwaltung = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<WorkDayViewModel> WorkDays { get; }
    }
}