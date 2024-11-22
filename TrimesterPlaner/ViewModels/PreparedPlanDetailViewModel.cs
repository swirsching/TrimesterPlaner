using TrimesterPlaner.Models;
using TrimesterPlaner.Utilities;

namespace TrimesterPlaner.ViewModels
{
    public class PreparedPlanDetailViewModel(string developer, PlanData plan) : BindableBase
    {
        public string Developer { get; } = developer;

        public DateTime Start { get; } = plan.RemainingPerDay.First().Key.Date;
        public DateTime End { get; } = plan.RemainingPerDay.Last().Key.Date;

        public double PlanPT { get; } = plan.PlanPT;
        public double ActualPT { get; } = Math.Round(plan.RemainingPerDay.First().Value - plan.RemainingPerDay.Last().Value, 1);

        public string Description { get; } = plan.TopLeft;
    }
}