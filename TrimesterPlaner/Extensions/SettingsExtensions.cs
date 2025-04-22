using TrimesterPlaner.Models;
using Utilities.Extensions;

namespace TrimesterPlaner.Extensions
{
    public static class SettingsExtensions
    {
        public static IEnumerable<DateTime> GetDaysOfEntwicklungsperiode(this Settings settings)
        {
            return CalendarHelpers.GetDaysBetweenDates(settings.Start, settings.Entwicklungsschluss);
        }
    }
}