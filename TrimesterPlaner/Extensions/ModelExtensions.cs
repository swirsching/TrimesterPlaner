using System;
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

        // Original | Remaining | Tracked | Result
        // A | 3 | 3 | 0 | 3
        // B | 3 | 5 | 0 | 5
        // C | 3 | 2 | 0 | 3
        // D | 3 | 0 | 0 | 3
        // E | 3 | 0 | 5 | 5
        // F | 3 | 0 | 2 | 2
        // G | 3 | 1 | 2 | 3
        // H | 3 | 1 | 1 | 2
        public static double GetTotalPT(this TimeEstimate timeEstimate)
        {
            double original = timeEstimate.OriginalEstimate ?? 0;
            double remaining = timeEstimate.RemainingEstimate ?? 0;
            double tracked = timeEstimate.TimeSpent ?? 0;

            if (original == 0 && remaining == 0 && tracked == 0 && timeEstimate is Ticket ticket)
            {
                return ticket.Shirt?.ToPT() ?? 0;
            }

            // G + H
            if (remaining > 0.0 && tracked > 0.0)
            {
                return remaining + tracked;
            }

            // D + E + F
            if (remaining <= 0.0)
            {
                // E + F
                if (tracked > 0.0)
                {
                    return tracked;
                }

                // D
                return original;
            }

            // A + B + C
            if (tracked <= 0.0)
            {
                return Math.Max(original, remaining);
            }

            // This cannot be reached
            return original;
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
}