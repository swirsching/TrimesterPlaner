using TrimesterPlaner.Models;

namespace TrimesterPlaner.ViewModels
{
    public class SpecialPlanViewModel(SpecialPlan plan, IEntwicklungsplanManager entwicklungsplanManager) : BaseViewModel(entwicklungsplanManager)
    {
        public string Description
        {
            get => plan.Description ?? "";
            set
            {
                plan.Description = value;
                OnPropertyChanged();
            }
        }

        public int Days
        {
            get => plan.Days;
            set
            {
                plan.Days = value;
                OnPropertyChanged();
            }
        }
    }
}