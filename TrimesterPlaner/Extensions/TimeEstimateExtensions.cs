using TrimesterPlaner.Models;

namespace TrimesterPlaner.Extensions
{
    public static class TimeEstimateExtensions
    {
        public static ShirtSize? ToShirtSize(this string shirt)
        {
            return shirt switch
            {
                "Mini" => ShirtSize.Mini,
                "XXS" => ShirtSize.XXS,
                "XS" => ShirtSize.XS,
                "S" => ShirtSize.S,
                "M" => ShirtSize.M,
                "L" => ShirtSize.L,
                "XL" => ShirtSize.XL,
                "XXL" => ShirtSize.XXL,
                _ => null,
            };
        }

        public static double ToPT(this ShirtSize shirt)
        {
            return shirt switch
            {
                ShirtSize.Mini => 1,
                ShirtSize.XXS => 3,
                ShirtSize.XS => 5,
                ShirtSize.S => 12,
                ShirtSize.M => 25,
                ShirtSize.L => 50,
                ShirtSize.XL => 100,
                ShirtSize.XXL => 200,
                _ => throw new NotImplementedException(),
            };
        }

        public static double GetTotalPT(this TimeEstimate timeEstimate)
        {
            double original = timeEstimate.OriginalEstimate ?? 0;
            double remaining = timeEstimate.RemainingEstimate ?? 0;
            double tracked = timeEstimate.TimeSpent ?? 0;

            if (original == 0 && remaining == 0 && tracked == 0 && timeEstimate is Ticket ticket)
            {
                return ticket.Shirt?.ToPT() ?? 0;
            }

            return tracked > 0.0 ? remaining + tracked : Math.Max(original, remaining);
        }
    }
}