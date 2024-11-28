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
        public string Description { get; } = plan.TopLeft;
    }
}