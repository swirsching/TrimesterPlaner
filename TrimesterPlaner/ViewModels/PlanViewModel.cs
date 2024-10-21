using System.Windows.Input;
using TrimesterPlaner.Models;
using TrimesterPlaner.Utilities;

namespace TrimesterPlaner.ViewModels
{
    public class PlanViewModel(Plan plan, IPlanManager planManager, IDeveloperProvider developerProvider, IEntwicklungsplanManager entwicklungsplanManager) : BaseViewModel(entwicklungsplanManager)
    {
        public Plan Plan { get; } = plan;

        private bool _IsChangingDeveloper = false;
        public bool IsChangingDeveloper
        {
            get => _IsChangingDeveloper;
            set => SetProperty(ref _IsChangingDeveloper, value);
        }

        public IEnumerable<Developer> Developers { get; } = developerProvider.GetDevelopers();

        public Developer? SelectedDeveloper
        {
            get => Plan.Developer;
            set
            {
                if (value is not null)
                {
                    Plan.Developer = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand MoveUpCommand { get; } = new RelayCommand((o) => planManager.MoveUp(plan));
        public ICommand MoveDownCommand { get; } = new RelayCommand((o) => planManager.MoveDown(plan));
        public ICommand RemoveCommand { get; } = new RelayCommand((o) => planManager.RemovePlan(plan));
    }
}