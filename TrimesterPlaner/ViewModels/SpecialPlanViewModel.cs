using TrimesterPlaner.Models;
using TrimesterPlaner.Utilities;

namespace TrimesterPlaner.ViewModels
{
    public class SpecialPlanViewModel(SpecialPlan plan) : BindableBase
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