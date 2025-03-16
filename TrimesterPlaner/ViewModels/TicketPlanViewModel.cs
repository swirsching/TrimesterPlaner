using TrimesterPlaner.Models;
using TrimesterPlaner.Utilities;

namespace TrimesterPlaner.ViewModels
{
    public class TicketPlanViewModel(TicketPlan plan) : BindableBase
    {
        public string Key { get => plan.Ticket?.Key ?? ""; }
        public string Summary { get => plan.Ticket?.Summary ?? ""; }

        public string Description
        {
            get => plan.Description;
            set
            {
                plan.Description = value;
                OnPropertyChanged();
            }
        }

        public bool HasTimeEstimateOverride
        {
            get => plan.TimeEstimateOverride is not null;
            set
            {
                if (value)
                {
                    double? original = OriginalEstimate;
                    double? remaining = RemainingEstimate;
                    double? spent = TimeSpent;

                    plan.TimeEstimateOverride = new();

                    OriginalEstimate = original;
                    RemainingEstimate = remaining;
                    TimeSpent = spent;
                }
                else
                {
                    OriginalEstimate = plan.Ticket?.OriginalEstimate;
                    RemainingEstimate = plan.Ticket?.RemainingEstimate;
                    TimeSpent = plan.Ticket?.TimeSpent;

                    plan.TimeEstimateOverride = null;
                }
                OnPropertyChanged();
            }
        }

        public double? OriginalEstimate
        {
            get => plan.TimeEstimateOverride?.OriginalEstimate ?? plan.Ticket?.OriginalEstimate;
            set
            {
                plan.TimeEstimateOverride!.OriginalEstimate = value;
                OnPropertyChanged();
            }
        }

        public double? RemainingEstimate
        {
            get => plan.TimeEstimateOverride?.RemainingEstimate ?? plan.Ticket?.RemainingEstimate;
            set
            {
                plan.TimeEstimateOverride!.RemainingEstimate = value;
                OnPropertyChanged();
            }
        }

        public double? TimeSpent
        {
            get => plan.TimeEstimateOverride?.TimeSpent ?? plan.Ticket?.TimeSpent;
            set
            {
                plan.TimeEstimateOverride!.TimeSpent = value;
                OnPropertyChanged();
            }
        }
    }
}