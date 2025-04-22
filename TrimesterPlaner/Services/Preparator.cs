using System.Globalization;
using TrimesterPlaner.Extensions;
using TrimesterPlaner.Models;
using Utilities.Extensions;

namespace TrimesterPlaner.Services
{
    public static class Widths
    {
        public static int Left { get; } = 40;
        public static int WeekDay { get; } = 50;
        public static int WeekEndDay { get; } = 5;
    }

    public interface IPreparator
    {
        public PreparedData? Prepare(Config config);
    }

    public record PreparedData(bool Burndown, DateTime Entwicklungsstart, DateTime Entwicklungsschluss, IEnumerable<Week> Weeks, IEnumerable<DeveloperData> Developers);
    public class Week(int weeknum)
    {
        public int Weeknum { get; } = weeknum;
        public List<Day> Days { get; } = [];
    }
    public class Day(DateTime date, bool isBadArea)
    {
        public DateTime Date { get; } = date;
        public bool IsBadArea { get; } = isBadArea;
        public double Capacity { get; set; } = 0.0;
        public double Total { get; set; } = 0.0;

        public int X { private get; set; } = 0;
        public int GetX(double alpha)
        {
            return (int)(X + Math.Clamp(alpha, 0, 1) * (this.IsWeekend() ? Widths.WeekEndDay : Widths.WeekDay));
        }
    }
    public record DayWithPT(Day Day, double Before, double After);
    public record DeveloperData(string Abbreviation, IEnumerable<Day> FreeDays, IEnumerable<DayWithPT> Days, IEnumerable<PlanData> Plans, IEnumerable<VacationData> Vacations);
    public record PlanData(int StartX, int EndX, int RemainingX, PlanType PlanType, string FirstRow, string SecondRow, string TopLeft, double StartPT, double EndPT);
    public enum PlanType { Ticket, Bug, Special };
    public record VacationData(IEnumerable<Day> Days, string Label);

    public class Preparator : IPreparator
    {
        public PreparedData? Prepare(Config config)
        {
            if (config.Settings is null)
            {
                return null;
            }

            if (config.Settings.Entwicklungsstart is null || config.Settings.Entwicklungsschluss is null)
            {
                return null;
            }

            if (config.Settings.Entwicklungsschluss <= config.Settings.Entwicklungsstart)
            {
                return null;
            }

            config.Settings.Start ??= config.Settings.Entwicklungsstart;

            List<Day> days = PrepareDays(
                config.Settings.Start.Value,
                config.Settings.Start.Value.AddYears(10),
                config.Settings.Entwicklungsschluss.Value);

            List<DeveloperData> developers = new(from developer in config.Developers
                                                 select PrepareDeveloper(days, developer));

            var lastPlannedDay = (from developer in developers
                                  select developer.Days.GetDay(developer.Plans.LastOrDefault()?.EndX ?? 0))
                                  .MaxBy(day => day?.Day.Date);
            days.RemoveAll(day => day.IsBadArea && (lastPlannedDay is null || day.Date > lastPlannedDay.Day.Date));

            IEnumerable<PlanData> allPlans = from developer in developers
                                             from plan in developer.Plans
                                             select plan;

            CalculateCapacity(days, config.Developers);
            CalculateTotal(days, allPlans);

            int weekCount = CalendarHelpers.GetWeeksBetweenDates(days.First().Date, days.Last().Date);
            List<Week> weeks = new(from weeknum in Enumerable.Range(0, weekCount)
                                   select new Week(GetWeeknum(days.First().Date.AddDays(7 * weeknum))));

            var weekEnumerator = weeks.GetEnumerator();
            weekEnumerator.MoveNext();
            foreach (Day day in days)
            {
                weekEnumerator.Current.Days.Add(day);
                if (day.Date.DayOfWeek == DayOfWeek.Sunday)
                {
                    weekEnumerator.MoveNext();
                }
            }

            return new(config.Settings.Burndown, config.Settings.Entwicklungsstart.Value, config.Settings.Entwicklungsschluss.Value, weeks, developers);
        }

        private static int GetWeeknum(DateTime date) => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstDay, DayOfWeek.Monday);

        private static void CalculateCapacity(List<Day> days, IEnumerable<Developer> developers)
        {
            double capacity = 0.0;
            for (int idx = days.Count - 1; idx >= 0; idx--)
            {
                if (!days[idx].IsBadArea)
                {
                    capacity += (from developer in developers
                                 where developer.IsWorkDay(days[idx].Date)
                                 select developer.GetDailyPT()).Sum();
                }
                days[idx].Capacity = (float)capacity;
            }
        }

        private static void CalculateTotal(List<Day> days, IEnumerable<PlanData> plans)
        {
            foreach (Day day in days)
            {
                foreach (PlanData plan in plans)
                {
                    //day.Total += (double)plan.GetRemainingAtDate(day.Date);
                }
            }
        }

        public static List<Day> PrepareDays(DateTime start, DateTime end, DateTime entwicklungsschluss)
        {
            List<Day> days = new(from date in CalendarHelpers.GetDaysBetweenDates(start, end)
                                 select new Day(date, date > entwicklungsschluss));

            int width = Widths.Left;
            foreach (var day in days)
            {
                day.X = width;
                width += day.Date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday ? Widths.WeekEndDay : Widths.WeekDay;
            }

            return days;
        }

        private static DeveloperData PrepareDeveloper(IEnumerable<Day> days, Developer developer)
        {
            IEnumerable<Day> freeDays = from day in days
                                        where !developer.IsRegularWorkDay(day.Date)
                                        select day;

            IEnumerable<DayWithPT> daysWithPT = PrepareDaysForDeveloper(days, developer);

            IEnumerable<PlanData> plans = PreparePlans(daysWithPT, developer.Plans);

            IEnumerable<VacationData> vacations = from vacation in developer.Vacations
                                                  where vacation.Start is not null
                                                  where vacation.End is not null
                                                  select PrepareVacation(days, vacation);

            return new(developer.Abbreviation, freeDays, daysWithPT, plans, vacations);
        }

        public static IEnumerable<DayWithPT> PrepareDaysForDeveloper(IEnumerable<Day> days, Developer developer)
        {
            List<DayWithPT> daysWithPT = [];
            double before = 0, after = 0;
            double dailyPT = developer.GetDailyPT();
            foreach (Day day in days)
            {
                if (developer.IsWorkDay(day.Date))
                {
                    after += dailyPT;
                }
                daysWithPT.Add(new(day, before, after));
                before = after;
            }
            return daysWithPT;
        }

        public static List<PlanData> PreparePlans(IEnumerable<DayWithPT> daysWithPT, IEnumerable<Plan> plans)
        {
            if (!plans.Any())
            {
                return [];
            }

            var maxPT = (from dayWithPT in daysWithPT
                         where !dayWithPT.Day.IsBadArea
                         select dayWithPT.After).Last();

            List<PlanData> data = [];
            double startPT = 0;
            foreach (var planWithPT in plans.Stretch(maxPT))
            {
                if (planWithPT.Plan.EarliestStart is not null)
                {
                    startPT = Math.Max(startPT, daysWithPT.GetDay(planWithPT.Plan.EarliestStart.Value)?.Before ?? startPT);
                }
                double endPT = startPT + planWithPT.PT;
                data.Add(PreparePlan(planWithPT.Plan, daysWithPT.GetX(startPT, PositionInPlan.Start), daysWithPT.GetX(endPT, PositionInPlan.End), startPT, endPT));
                startPT = endPT;
            }

            return data;
        }

        private static PlanData PreparePlan(Plan plan, int startX, int endX, double startPT, double endPT)
        {
            Ticket? ticket = (plan as TicketPlan)?.Ticket;

            int remainingX = startX;
            double? remaining = (plan as TicketPlan)?.TimeEstimateOverride?.RemainingEstimate ?? ticket?.RemainingEstimate;
            if (remaining is not null)
            {
                double totalPT = endPT - startPT;
                double alpha = (totalPT - remaining.Value) / totalPT;
                remainingX += (int)Math.Round(alpha * (endX - startX));
            }

            return new(
                startX,
                endX,
                remainingX,
                GetPlanType(plan),
                ticket?.Key ?? (plan as SpecialPlan)?.Description ?? "",
                ticket?.Summary ?? "",
                (plan as TicketPlan)?.Description ?? "",
                startPT,
                endPT);
        }

        private static PlanType GetPlanType(Plan plan)
        {
            if (plan is BugPlan)
            {
                return PlanType.Bug;
            }
            if (plan is SpecialPlan)
            {
                return PlanType.Special;
            }
            return PlanType.Ticket;
        }

        private static VacationData PrepareVacation(IEnumerable<Day> days, Vacation vacation)
        {
            var vacationDays = from day in days
                               where vacation.IsInside(day.Date)
                               select day;
            return new(vacationDays, vacation.Label);
        }
    }
}