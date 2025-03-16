using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using TrimesterPlaner.Extensions;
using TrimesterPlaner.Models;
using TrimesterPlaner.Providers;
using TrimesterPlaner.Utilities;

namespace TrimesterPlaner.ViewModels
{
    public class PlanProviderViewModel : BindableBase
    {
        public PlanProviderViewModel(DeveloperProviderViewModel developerProviderViewModel)
        {
            Plans = new() { Source = Inject.GetCollection<Plan>() };
            Plans.Filter += FilterBySelectedDeveloper;
            developerProviderViewModel.OnSelectedDeveloperChanged += OnSelectedDeveloperChanged;

            AddBugPlanCommand = new RelayCommand((o) => Inject.Require<IPlanProvider>().AddBugPlan(SelectedDeveloper!));
            AddSpecialPlanCommand = new RelayCommand((o) => Inject.Require<IPlanProvider>().AddSpecialPlan(SelectedDeveloper!));
        }

        private void FilterBySelectedDeveloper(object sender, FilterEventArgs e)
        {
            e.Accepted = SelectedDeveloper is null || SelectedDeveloper == ((Plan)e.Item).Developer;
        }

        private void OnSelectedDeveloperChanged(Developer? selectedDeveloper)
        {
            SelectedDeveloper = selectedDeveloper;
            PlansView.Refresh();
        }

        private Developer? _SelectedDeveloper;
        public Developer? SelectedDeveloper
        {
            get => _SelectedDeveloper;
            set => SetProperty(ref _SelectedDeveloper, value);
        }

        private CollectionViewSource Plans { get; }

        public ICollectionView PlansView
        {
            get => Plans.View;
        }

        public ICommand AddBugPlanCommand { get; }
        public ICommand AddSpecialPlanCommand { get; }
    }
}