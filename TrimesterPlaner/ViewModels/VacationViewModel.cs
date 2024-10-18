using System.Windows.Input;
using TrimesterPlaner.Models;
using TrimesterPlaner.Utilities;

namespace TrimesterPlaner.ViewModels
{
    public class VacationViewModel(Vacation vacation, IVacationManager vacationManager, IEntwicklungsplanManager entwicklungsplanManager) : BaseViewModel(entwicklungsplanManager)
    {
        public DateTime? Start
        {
            get => vacation.Start;
            set
            {
                vacation.Start = value;
                End ??= value;
                OnPropertyChanged();
            }
        }

        public DateTime? End
        {
            get => vacation.End;
            set
            {
                vacation.End = value;
                OnPropertyChanged();
            }
        }

        public string Label
        {
            get => vacation.Label;
            set
            {
                vacation.Label = value;
                OnPropertyChanged();
            }
        }

        public string? Developer
        {
            get => vacation.Developer?.Abbreviation;
        }

        public ICommand RemoveCommand { get; } = new RelayCommand((o) => vacationManager.RemoveVacation(vacation));
    }
}