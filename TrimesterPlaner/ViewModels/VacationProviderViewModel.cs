using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using TrimesterPlaner.Models;
using TrimesterPlaner.Providers;
using TrimesterPlaner.Utilities;

namespace TrimesterPlaner.ViewModels
{
    public class VacationProviderViewModel : BindableBase
    {
        public VacationProviderViewModel(
            IVacationProvider vacationProvider,
            DeveloperProviderViewModel developerProviderViewModel) : base()
        {
            VacationsViewSource = new() { Source = vacationProvider.GetAll() };
            VacationsViewSource.Filter += FilterBySelectedDeveloper;
            developerProviderViewModel.OnSelectedDeveloperChanged += SelectedDeveloperChanged;

            AddVacationCommand = new RelayCommand((o) => vacationProvider.AddVacation(SelectedDeveloper!));
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

        private CollectionViewSource VacationsViewSource { get; }

        public ICollectionView VacationsView
        {
            get => VacationsViewSource.View;
        }

        public ICommand AddVacationCommand { get; }
    }
}