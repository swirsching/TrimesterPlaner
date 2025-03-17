using TrimesterPlaner.Models;
using TrimesterPlaner.Utilities;

namespace TrimesterPlaner.ViewModels
{
    public class BugPlanViewModel(BugPlan plan) : BindableBase
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