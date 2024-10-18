using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using TrimesterPlaner.Models;
using TrimesterPlaner.Utilities;

namespace TrimesterPlaner.ViewModels
{
    public interface IPlanProvider
    {
        public IEnumerable<Plan> GetPlans();
    }

    public class PlanProviderViewModel : BindableBase
    {
        public PlanProviderViewModel(IPlanProvider planProvider, IPlanManager planManager, DeveloperProviderViewModel developerProviderViewModel) : base()
        {
            Plans = new() { Source = planProvider.GetPlans() };
            Plans.Filter += FilterBySelectedDeveloper;
            developerProviderViewModel.OnSelectedDeveloperChanged += OnSelectedDeveloperChanged;

            AddBugPlanCommand = new RelayCommand((o) => planManager.AddBugPlan(SelectedDeveloper!));
            AddSpecialPlanCommand = new RelayCommand((o) => planManager.AddSpecialPlan(SelectedDeveloper!));
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