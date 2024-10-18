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

        public bool HasPT
        {
            get => plan.PT is not null;
            set
            {
                plan.PT = value ? 0 : null;
                OnPropertyChanged();
            }
        }

        public double? PT
        {
            get => plan.PT;
            set
            {
                plan.PT = value;
                OnPropertyChanged();
            }
        }

        public bool HasStartAndEnd
        {
            get => plan.Start is not null && plan.End is not null;
            set
            {
                plan.Start = value ? DateTime.Now : null;
                plan.End = value ? DateTime.Now : null;
                OnPropertyChanged();
            }
        }

        public DateTime? Start
        {
            get => plan.Start;
            set
            {
                plan.Start = value;
                OnPropertyChanged();
            }
        }

        public DateTime? End
        {
            get => plan.End;
            set
            {
                plan.End = value;
                OnPropertyChanged();
            }
        }
    }
}