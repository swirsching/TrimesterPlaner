using TrimesterPlaner.Models;
using TrimesterPlaner.Services;

namespace TrimesterPlaner.ViewModels
{
    public class BugPlanViewModel(BugPlan plan, IPlaner trimesterPlaner) : BaseViewModel(trimesterPlaner)
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