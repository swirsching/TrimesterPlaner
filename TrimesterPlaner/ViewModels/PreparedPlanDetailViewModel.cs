using TrimesterPlaner.Extensions;
using TrimesterPlaner.Models;
using TrimesterPlaner.Utilities;

namespace TrimesterPlaner.ViewModels
{
    public class PreparedPlanDetailViewModel(DeveloperData developer, PlanData plan) : BindableBase
    {
        public string Developer { get; } = developer.Abbreviation;
        public DateTime Start { get; } = developer.Days.GetDay(plan.StartX)!.Day.Date;
        public DateTime End { get; } = developer.Days.GetDay(plan.EndX)!.Day.Date;
        public double PlanPT { get; } = 0; // TODO: Math.Round(plan.EndPT - plan.StartPT, 1);
        public string Description { get; } = plan.TopLeft;
    }
}