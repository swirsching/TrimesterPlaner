using TrimesterPlaner.Models;
using TrimesterPlaner.Services;

namespace TrimesterPlaner.ViewModels
{
    public class SpecialPlanViewModel(SpecialPlan plan, IPlaner trimesterPlaner) : BaseViewModel(trimesterPlaner)
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