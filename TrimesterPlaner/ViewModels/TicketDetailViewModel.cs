using TrimesterPlaner.Extensions;
using TrimesterPlaner.Models;
using TrimesterPlaner.Utilities;

namespace TrimesterPlaner.ViewModels
{
    public class TicketDetailViewModel : BindableBase
    {
        public TicketDetailViewModel(Ticket ticket, IEntwicklungsplanManager entwicklungsplanManager)
        {
            Ticket = ticket;
            TotalPT = ticket.GetTotalPT();

            entwicklungsplanManager.EntwicklungsplanChanged += CalculatePlanDetails;
        }

        private void CalculatePlanDetails(PreparedData? data)
        {
            PlannedPT = Ticket.GetPlannedPT();
        }

        public Ticket Ticket { get; }
        public double TotalPT { get; }

        private double _PlannedPT;
        public double PlannedPT 
        { 
            get => _PlannedPT;
            set => SetProperty(ref _PlannedPT, value);
        }
    }
}