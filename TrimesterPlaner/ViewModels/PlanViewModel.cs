using System.Windows.Input;
using TrimesterPlaner.Models;
using TrimesterPlaner.Utilities;

namespace TrimesterPlaner.ViewModels
{
    public class PlanViewModel(Plan plan, IPlanManager planManager, IEntwicklungsplanManager entwicklungsplanManager) : BaseViewModel(entwicklungsplanManager)
    {
        public Plan Plan { get => plan; }

        public ICommand MoveUpCommand { get; } = new RelayCommand((o) => planManager.MoveUp(plan));
        public ICommand MoveDownCommand { get; } = new RelayCommand((o) => planManager.MoveDown(plan));
        public ICommand RemoveCommand { get; } = new RelayCommand((o) => planManager.RemovePlan(plan));
    }
}