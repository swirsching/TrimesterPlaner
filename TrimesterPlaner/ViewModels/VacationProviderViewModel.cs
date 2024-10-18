using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using TrimesterPlaner.Models;
using TrimesterPlaner.Utilities;

namespace TrimesterPlaner.ViewModels
{
    public interface IVacationProvider
    {
        public IEnumerable<Vacation> GetVacations();
    }

    public class VacationProviderViewModel : BindableBase
    {
        public VacationProviderViewModel(IVacationManager vacationManager, IVacationProvider vacationProvider, DeveloperProviderViewModel developerProviderViewModel) : base()
        {
            Vacations = new() { Source = vacationProvider.GetVacations() };
            Vacations.Filter += FilterBySelectedDeveloper;
            developerProviderViewModel.OnSelectedDeveloperChanged += SelectedDeveloperChanged;

            AddVacationCommand = new RelayCommand((o) => vacationManager.AddVacation(SelectedDeveloper!));
        }

        private void FilterBySelectedDeveloper(object sender, FilterEventArgs e)
        {
            e.Accepted = SelectedDeveloper is null || SelectedDeveloper == ((Vacation)e.Item).Developer;
        }

        private void SelectedDeveloperChanged(Developer? selectedDeveloper)
        {
            SelectedDeveloper = selectedDeveloper;
            VacationsView.Refresh();
        }

        private Developer? _SelectedDeveloper;
        public Developer? SelectedDeveloper
        {
            get => _SelectedDeveloper;
            set => SetProperty(ref _SelectedDeveloper, value);
        }

        private CollectionViewSource Vacations { get; }

        public ICollectionView VacationsView
        {
            get => Vacations.View;
        }

        public ICommand AddVacationCommand { get; }
    }
}