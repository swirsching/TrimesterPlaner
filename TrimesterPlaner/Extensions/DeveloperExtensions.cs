using TrimesterPlaner.Models;

namespace TrimesterPlaner.Extensions
{
    public static class DeveloperExtensions
    {
        public static bool IsRegularWorkDay(this Developer developer, DateTime date)
        {
            if (developer.FreeDays.Contains(date.DayOfWeek))
            {
                return false;
            }

            return true;
        }

        public static bool IsWorkDay(this Developer developer, DateTime date)
        {
            if (!developer.IsRegularWorkDay(date))
            {
                return false;
            }

            foreach (Vacation vacation in developer.Vacations)
            {
                if (vacation.IsInside(date))
                {
                    return false;
                }
            }

            return true;
        }

        public static double GetDailyPT(this Developer developer)
        {
            double fte = developer.FTE / 100.0;
            double sonderrolle = developer.Sonderrolle / 100.0;
            double verwaltung = developer.Verwaltung / 100.0;
            return fte * (1.0 - sonderrolle - verwaltung);
        }
    }
}