using TrimesterPlaner.Models;

namespace TrimesterPlaner.Extensions
{
    public static class TicketExtensions
    {
        public static double GetPlannedPT(this Ticket ticket)
        {
            double ticketPT = ticket.GetTotalPT();
            double plannedPT = (from plan in ticket.Plans
                                select plan?.TimeEstimateOverride?.GetTotalPT() ?? ticketPT).Sum();
            return plannedPT;
        }
    }
}