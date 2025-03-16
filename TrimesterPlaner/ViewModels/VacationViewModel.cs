using System.Windows.Input;
using TrimesterPlaner.Models;
using TrimesterPlaner.Providers;
using TrimesterPlaner.Services;
using TrimesterPlaner.Utilities;

namespace TrimesterPlaner.ViewModels
{
    public class VacationViewModel(
        Vacation vacation,
        IVacationProvider vacationProvider,
        IPlaner trimesterPlaner) : BaseViewModel(trimesterPlaner)
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

        public ICommand RemoveCommand { get; } = new RelayCommand((o) => vacationProvider.Remove(vacation));
    }
}