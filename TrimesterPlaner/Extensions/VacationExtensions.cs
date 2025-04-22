using TrimesterPlaner.Models;
using Utilities.Extensions;

namespace TrimesterPlaner.Extensions
{
    public static class VacationExtensions
    {
        public static bool IsInside(this Vacation vacation, DateTime date)
        {
            return date >= vacation.Start && date <= vacation.End;
        }

        public static IEnumerable<DateTime> GetDays(this Vacation vacation)
        {
            return CalendarHelpers.GetDaysBetweenDates(vacation.Start, vacation.End);
        }
    }
}
