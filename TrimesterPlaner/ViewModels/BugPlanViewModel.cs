using TrimesterPlaner.Models;

namespace TrimesterPlaner.ViewModels
{
    public class BugPlanViewModel(BugPlan plan, IEntwicklungsplanManager entwicklungsplanManager) : BaseViewModel(entwicklungsplanManager)
    {
        public double PT
        {
            get => plan.PT;
            set
            {
                plan.PT = value;
                OnPropertyChanged();
            }
        }
    }
}