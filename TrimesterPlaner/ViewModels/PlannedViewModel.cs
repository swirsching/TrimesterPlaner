using TrimesterPlaner.Extensions;
using TrimesterPlaner.Models;
using TrimesterPlaner.Utilities;

namespace TrimesterPlaner.ViewModels
{
    public class PlannedViewModel : BindableBase
    {
        public PlannedViewModel(Ticket ticket, IEntwicklungsplanManager entwicklungsplanManager)
        {
            Ticket = ticket;

            entwicklungsplanManager.EntwicklungsplanChanged += CalculatePlannedPercentage;
            CalculatePlannedPercentage();
        }

        private void CalculatePlannedPercentage(PreparedData? data = null)
        {
            double ticketPT = Ticket.GetTotalPT();
            double plannedPT = (from plan in Ticket.Plans
                                select plan?.TimeEstimateOverride?.GetTotalPT() ?? ticketPT).Sum();
            PlannedPercentage = plannedPT / ticketPT;
        }

        private Ticket Ticket { get; }

        private double _PlannedPercentage;
        public double PlannedPercentage
        {
            get => _PlannedPercentage;
            set => SetProperty(ref _PlannedPercentage, value);
        }
    }
}