using TrimesterPlaner.Models;
using TrimesterPlaner.Services;

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

    public record PlanWithPT(Plan Plan, double PT);
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

        public static IEnumerable<PlanWithPT> Stretch(this IEnumerable<Plan> plans, double maxPT)
        {
            double totalPT = (from plan in plans
                              select plan.GetTotalPT()).Sum();

            var stretchablePlans = from plan in plans
                                   where plan.IsStretchable()
                                   select plan;

            double stretchPT = totalPT > maxPT ? 0 : (maxPT - totalPT) / stretchablePlans.Count();

            var stretchedPlans = from plan in plans
                                 select new PlanWithPT(plan, plan.IsStretchable() ? stretchPT : plan.GetTotalPT());
            return stretchedPlans;
        }

        private static bool IsStretchable(this Plan plan)
        {
            return plan is BugPlan bugPlan && bugPlan.PT == 0;
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