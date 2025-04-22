using TrimesterPlaner.Services;
using Utilities.Extensions;

namespace TrimesterPlaner.Extensions
{
    public enum PositionInDay { Front, Middle, Back }
    public enum PositionInPlan { Start, End }
    public static class PreparedDataExtensions
    {
        public static bool IsWeekend(this Day day)
        {
            return day.Date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;
        }

        public static int GetX(this IEnumerable<Week> weeks, DateTime date, PositionInDay positionInDay)
        {
            var days = from week in weeks
                       from d in week.Days
                       where d.Date.IsSameDayAs(date)
                       select d;

            if (days.Any())
            {
                double alpha = positionInDay switch
                {
                    PositionInDay.Front => 0,
                    PositionInDay.Middle => 0.5,
                    PositionInDay.Back => 1,
                    _ => throw new NotImplementedException(),
                };
                return days.First().GetX(alpha);
            }

            return 0;
        }

        public static int GetX(this IEnumerable<DayWithPT> days, double pt, PositionInPlan positionInPlan)
        {
            double epsilon = 0.1;
            var relevantDays = from day in days
                               where day.Before - epsilon <= pt && pt <= day.After + epsilon
                               select day;
            if (!relevantDays.Any())
            {
                return 0;
            }

            if (relevantDays.Count() == 1)
            {
                return relevantDays.First().GetX(pt, positionInPlan);
            }

            return positionInPlan switch
            {
                PositionInPlan.Start => relevantDays.Last().GetX(pt, positionInPlan),
                PositionInPlan.End => relevantDays.First().GetX(pt, positionInPlan),
                _ => throw new NotImplementedException()
            };
        }

        public static DayWithPT? GetDay(this IEnumerable<DayWithPT> days, int x)
        {
            return days.FirstOrDefault((d) => d.Day.GetX(0) <= x && x <= d.Day.GetX(1));
        }

        public static DayWithPT? GetDay(this IEnumerable<DayWithPT> days, DateTime date)
        {
            return days.FirstOrDefault((d) => d.Day.Date == date);
        }

        private static int GetX(this DayWithPT day, double pt, PositionInPlan positionInPlan)
        {
            var alpha = GetAlpha(day.Before, day.After, pt);
            if (alpha > 0.9 && positionInPlan == PositionInPlan.End)
            {
                alpha = 1.0;
            }
            else if (alpha < 0.1 && positionInPlan == PositionInPlan.Start)
            {
                alpha = 0.0;
            }
            return day.Day.GetX(alpha);
        }

        public static double GetAlpha(double before, double after, double pt)
        {
            return (pt - before) / (after - before);
        }

        public static double GetPT(this VacationData vacation, DeveloperData developer, Day? day = null)
        {
            if (day is null)
            {
                return (from d in vacation.Days
                        select vacation.GetPT(developer, d)).Sum();
            }

            if (!vacation.Days.Contains(day))
            {
                return 0;
            }

            if (developer.FreeDays.Contains(day))
            {
                return 0;
            }

            var firstWorkDay = developer.Days.FirstOrDefault((d) => d.Before != d.After);
            if (firstWorkDay is null)
            {
                return 0;
            }

            return firstWorkDay.After - firstWorkDay.Before;
        }

        public static double GetPT(this PlanData plan, DeveloperData developer, Day day)
        {
            var dayWithPT = developer.Days.FirstOrDefault((d) => d.Day == day);
            var startDay = developer.Days.GetDay(plan.StartX);
            var endDay = developer.Days.GetDay(plan.EndX);
            if (dayWithPT is null || startDay is null || endDay is null)
            {
                return 0;
            }

            if (day.Date < startDay.Day.Date)
            {
                return 0;
            }
            if (day.Date > endDay.Day.Date)
            {
                return 0;
            }

            if (day == startDay.Day)
            {
                return startDay.After - plan.StartPT;
            }
            if (day == endDay.Day)
            {
                return plan.EndPT - endDay.Before;
            }

            return dayWithPT.After - dayWithPT.Before;
        }
    }
}