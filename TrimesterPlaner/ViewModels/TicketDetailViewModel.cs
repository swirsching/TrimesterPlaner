using System.Collections.ObjectModel;
using TrimesterPlaner.Extensions;
using TrimesterPlaner.Models;
using TrimesterPlaner.Services;
using TrimesterPlaner.Utilities;

namespace TrimesterPlaner.ViewModels
{
    public class TicketDetailViewModel : BindableBase
    {
        public TicketDetailViewModel(Ticket ticket, IPlaner trimesterPlaner)
        {
            Ticket = ticket;
            TotalPT = ticket.GetTotalPT();
            PlanDetails = [];

            trimesterPlaner.PlanChanged += (data, result) => CalculatePlanDetails(data);
            CalculatePlanDetails(trimesterPlaner.GetLastPreparedData());
        }

        private void CalculatePlanDetails(PreparedData? data)
        {
            PlannedPT = Ticket.GetPlannedPT();
            if (data is null)
            {
                return;
            }

            var plans = from developer in data.Developers
                        from plan in developer.Plans
                        where plan.PlanType == PlanType.Ticket
                        where plan.FirstRow == Ticket.Key
                        select new PreparedPlanDetailViewModel(developer, plan);
            PlanDetails.ClearAndAdd(plans);
        }

        public Ticket Ticket { get; }
        public double TotalPT { get; }
        public ObservableCollection<PreparedPlanDetailViewModel> PlanDetails { get; }

        private double _PlannedPT;
        public double PlannedPT 
        { 
            get => _PlannedPT;
            set => SetProperty(ref _PlannedPT, value);
        }
    }
}