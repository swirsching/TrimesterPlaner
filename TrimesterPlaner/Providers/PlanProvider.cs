using System.Collections.ObjectModel;
using TrimesterPlaner.Extensions;
using TrimesterPlaner.Models;
using TrimesterPlaner.Services;

namespace TrimesterPlaner.Providers
{
    public interface IPlanProvider : ICollectionProvider<Plan>
    {
        public void AddTicketPlan(Developer developer, Ticket ticket);
        public void AddBugPlan(Developer developer);
        public void AddSpecialPlan(Developer developer);
        public void MoveUp(Plan plan);
        public void MoveDown(Plan plan);
        public void RemovePlans(Developer developer);
        public void RemovePlans(Ticket ticket);
    }

    public class PlanProvider(IPlaner planer) : IPlanProvider
    {
        private ObservableCollection<Plan> Plans { get; } = [];

        public IEnumerable<Plan> Get()
        {
            return Plans;
        }

        public void Set(IEnumerable<Plan> values)
        {
            Plans.ClearAndAdd(values);
        }

        public void Remove(Plan value)
        {
            RemovePlan(value);
        }

        private void AddPlan(Plan plan)
        {
            Plans.Add(plan);
            planer.RefreshPlan();
        }

        public void AddTicketPlan(Developer developer, Ticket ticket) => AddPlan(new TicketPlan() { Developer = developer, Ticket = ticket });
        public void AddBugPlan(Developer developer) => AddPlan(new BugPlan() { Developer = developer });
        public void AddSpecialPlan(Developer developer) => AddPlan(new SpecialPlan() { Developer = developer });

        private void ReorderPlansForDeveloper(Developer developer)
        {
            developer.Plans.Clear();
            foreach (Plan plan in Plans)
            {
                if (plan.Developer == developer)
                {
                    developer.Plans.Add(plan);
                }
            }
        }

        public void MoveUp(Plan plan)
        {
            int currentIdx = Plans.IndexOf(plan);
            for (int idx = currentIdx - 1; idx >= 0; idx--)
            {
                if (plan.Developer == Plans[idx].Developer)
                {
                    Plans.Move(currentIdx, idx);
                    ReorderPlansForDeveloper(plan.Developer!);
                    planer.RefreshPlan();
                    return;
                }
            }
        }

        public void MoveDown(Plan plan)
        {
            int currentIdx = Plans.IndexOf(plan);
            for (int idx = currentIdx + 1; idx < Plans.Count; idx++)
            {
                if (plan.Developer == Plans[idx].Developer)
                {
                    Plans.Move(currentIdx, idx);
                    ReorderPlansForDeveloper(plan.Developer!);
                    planer.RefreshPlan();
                    return;
                }
            }
        }

        public void RemovePlans(Developer developer)
        {
            List<Plan> plansToRemove = [.. developer.Plans];
            foreach (var plan in plansToRemove)
            {
                RemovePlan(plan, false);
            }
            planer.RefreshPlan();
        }

        public void RemovePlans(Ticket ticket)
        {
            List<Plan> plansToRemove = [.. ticket.Plans];
            foreach (Plan plan in plansToRemove)
            {
                RemovePlan(plan, false);
            }
            planer.RefreshPlan();
        }

        private void RemovePlan(Plan plan, bool refresh = true)
        {
            plan.Developer = null;
            if (plan is TicketPlan ticketPlan)
            {
                ticketPlan.Ticket = null;
            }
            Plans.Remove(plan);
            if (refresh)
            {
                planer.RefreshPlan();
            }
        }
    }
}