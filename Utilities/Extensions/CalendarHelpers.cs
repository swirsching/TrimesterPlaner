namespace Utilities.Extensions
{
    public static class CalendarHelpers
    {
        public static int GetWeeksBetweenDates(DateTime a, DateTime b)
        {
            int weeks = (int)Math.Ceiling((double)(b - a).TotalDays / 7);
            if (b.GetDayOfWeekStartingMonday() <= a.GetDayOfWeekStartingMonday())
            {
                weeks++;
            }
            return weeks;
        }

        public static int GetDayOfWeekStartingMonday(this DateTime date)
        {
            if (date.DayOfWeek == DayOfWeek.Sunday)
            {
                return 7;
            }
            return (int)date.DayOfWeek;
        }

        public static bool IsSameDayAs(this DateTime date, DateTime other)
        {
            return date.Day == other.Day && date.Month == other.Month && date.Year == other.Year;
        }

        public static IEnumerable<DateTime> GetDaysBetweenDates(DateTime? start, DateTime? end)
        {
            if (start is null || end is null)
            {
                return [];
            }

            List<DateTime> days = [];
            for (DateTime date = (DateTime)start; date <= end; date = date.AddDays(1))
            {
                days.Add(date);
            }
            return days;
        }
    }
}