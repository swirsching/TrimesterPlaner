using TrimesterPlaner.Models;

namespace TrimesterPlaner.Extensions
{
    public static class Helpers
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

    public static class SettingsExtensions
    {
        public static IEnumerable<DateTime> GetDaysOfEntwicklungsperiode(this Settings settings)
        {
            return Helpers.GetDaysBetweenDates(settings.Start, settings.Entwicklungsschluss);
        }
    }

    public static class VacationExtensions
    {
        public static bool IsInside(this Vacation vacation, DateTime date)
        {
            return date >= vacation.Start && date <= vacation.End;
        }

        public static IEnumerable<DateTime> GetDays(this Vacation vacation)
        {
            return Helpers.GetDaysBetweenDates(vacation.Start, vacation.End);
        }
    }

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

    public static class PlanExtensions
    {
        public static double GetTotalPT(this Plan plan)
        {
            if (plan is TicketPlan ticketPlan)
            {
                return ticketPlan.GetTotalPT();
            }
            else if (plan is BugPlan bugPlan)
            {
                return bugPlan.PT;
            }
            else if (plan is SpecialPlan specialPlan)
            {
                return specialPlan.Days * plan.Developer!.GetDailyPT();
            }
            return 0;
        }

        public static double GetTotalPT(this TicketPlan plan)
        {
            if (plan.TimeEstimateOverride is not null)
            {
                return plan.TimeEstimateOverride.GetTotalPT();
            }

            return plan.Ticket?.GetTotalPT() ?? 0;
        }
    }

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

    public static class PreparedDataExtensions
    {
        public static bool IsWeekend(this Day day)
        {
            return day.Date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;
        }

        public static int GetStartX(this PlanData plan)
        {
            if (plan.RemainingPerDay.Count == 0)
            {
                return -1;
            }

            return plan.RemainingPerDay.First().Key.GetX(0);
        }

        public static int GetRemainingX(this PlanData plan)
        {
            if (plan.RemainingPerDay.Count == 0 || plan.RemainingPT is null)
            {
                return -1;
            }

            var daysWithBiggerRemaining = from dayAndRemaining in plan.RemainingPerDay
                                          where dayAndRemaining.Value > plan.RemainingPT
                                          select dayAndRemaining;
            var daysToConsider = plan.RemainingPerDay.Take(daysWithBiggerRemaining.Count() + 1);
            return daysToConsider.Last().Key.GetX(daysToConsider.GetAlpha(plan.PlanPT, plan.RemainingPT!.Value));
        }

        public static int GetEndX(this PlanData plan)
        {
            if (plan.RemainingPerDay.Count == 0)
            {
                return -1;
            }

            return plan.RemainingPerDay.Last().Key.GetX(plan.RemainingPerDay.GetAlpha(plan.PlanPT));
        }

        private static double GetAlpha(this IEnumerable<KeyValuePair<Day, double>> remainingPerDay, double planPT, double remainingPT = 0)
        {
            // Example 1: Last day goes from 0.25 to -0.75 PT => Alpha should be 0.25
            // Example 2: Last day goes from 0.75 to -0.25 PT => Alpha should be 0.75
            double finalPT = remainingPerDay.Last().Value;
            double lastDayPT = (remainingPerDay.Count() > 1 ? remainingPerDay.ElementAt(remainingPerDay.Count() - 2).Value : planPT) - finalPT;
            double overshotPT = remainingPT - finalPT;
            return 1 - (overshotPT / lastDayPT);
        }
    }
}