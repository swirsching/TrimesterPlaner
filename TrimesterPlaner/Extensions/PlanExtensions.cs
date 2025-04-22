using TrimesterPlaner.Models;

namespace TrimesterPlaner.Extensions
{
    public record PlanWithPT(Plan Plan, double PT);
    public static class PlanExtensions
    {
        public static double GetTotalPT(this Plan plan)
        {
            if (plan is TicketPlan ticketPlan)
            {
                return ticketPlan.GetTotalPT();
            }
            else if (plan is BugPlan bugPlan)
            {
                return bugPlan.PT;
            }
            else if (plan is SpecialPlan specialPlan)
            {
                return specialPlan.Days * plan.Developer!.GetDailyPT();
            }
            return 0;
        }

        public static double GetTotalPT(this TicketPlan plan)
        {
            if (plan.TimeEstimateOverride is not null)
            {
                return plan.TimeEstimateOverride.GetTotalPT();
            }

            return plan.Ticket?.GetTotalPT() ?? 0;
        }

        public static IEnumerable<PlanWithPT> Stretch(this IEnumerable<Plan> plans, double maxPT)
        {
            double totalPT = (from plan in plans
                              select plan.GetTotalPT()).Sum();

            var stretchablePlans = from plan in plans
                                   where plan.IsStretchable()
                                   select plan;

            double stretchPT = totalPT > maxPT ? 0 : (maxPT - totalPT) / stretchablePlans.Count();

            var stretchedPlans = from plan in plans
                                 select new PlanWithPT(plan, plan.IsStretchable() ? stretchPT : plan.GetTotalPT());
            return stretchedPlans;
        }

        private static bool IsStretchable(this Plan plan)
        {
            return plan is BugPlan bugPlan && bugPlan.PT == 0;
        }
    }
}