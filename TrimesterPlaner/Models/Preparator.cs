using System.Globalization;
using TrimesterPlaner.Extensions;

namespace TrimesterPlaner.Models
{
    public interface IPreparator
    {
        public PreparedData? Prepare(Config config);
    }

    public record PreparedData(bool Fehlerteam, bool Burndown, DateTime Entwicklungsstart, DateTime Entwicklungsschluss, IEnumerable<Week> Weeks, IEnumerable<DeveloperData> Developers);
    public class Week(int weeknum)
    {
        public int Weeknum { get; } = weeknum;
        public List<Day> Days { get; } = [];
    }
    public class Day(DateTime date, bool isBadArea)
    {
        public DateTime Date { get; } = date;
        public bool IsBadArea { get; } = isBadArea;
        public int X { get; set; } = 0;
        public double Capacity { get; set; } = 0.0;
        public double Total { get; set; } = 0.0;
        public double Promised { get; set; } = 0.0;
        public Dictionary<PlanData, double> Plans { get; } = [];
        public Dictionary<VacationData, double> Vacations { get; } = [];
    }
    public record DeveloperData(string Abbreviation, IEnumerable<Day> FreeDays, IEnumerable<PlanData> Plans, IEnumerable<VacationData> Vacations);
    public class PlanData
    {
        public PlanData(PlanType planType, Dictionary<Day, double> remainingPerDay, double? remainingPT, string firstRow, string secondRow, string topLeft, bool promised)
        {
            PlanType = planType;
            RemainingPerDay = remainingPerDay;
            RemainingPT = remainingPT;
            FirstRow = firstRow;
            SecondRow = secondRow; 
            TopLeft = topLeft;
            Promised = promised;

            for (int idx = 0; idx < RemainingPerDay.Count; idx++)
            {
                var (day, remaining) = RemainingPerDay.ElementAt(idx);
                double diff = idx > 0
                    ? remainingPerDay.ElementAt(idx - 1).Value - remaining
                    : remaining - (idx + 1 < RemainingPerDay.Count ? RemainingPerDay.ElementAt(idx + 1).Value : 0);
                day.Plans.Add(this, diff);
            }
        }

        public PlanType PlanType { get; }
        public Dictionary<Day, double> RemainingPerDay { get; }
        public double? RemainingPT { get; }
        public string FirstRow { get; }
        public string SecondRow { get; }
        public string TopLeft { get; }
        public bool Promised { get; }
    }
    public enum PlanType { Ticket, Bug, Special };
    public class VacationData
    {
        public VacationData(Dictionary<Day, double> ptPerDay, string label)
        {
            Days = from element in ptPerDay select element.Key;
            Label = label;

            foreach (var element in ptPerDay)
            {
                element.Key.Vacations.Add(this, element.Value);
            }
        }

        public IEnumerable<Day> Days { get; }
        public string Label { get; }
    }

    public class Preparator() : IPreparator
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

            List<Day> days = new(from date in Helpers.GetDaysBetweenDates(config.Settings.Start.Value, config.Settings.Start.Value.AddYears(1))
                                 select new Day(date, date > config.Settings.Entwicklungsschluss.Value));

            List<DeveloperData> developers = new(from developer in config.Developers
                                                 select PrepareDeveloper(days, developer));

            days.RemoveAll(day => day.IsBadArea && day.Plans.Count == 0);

            IEnumerable<PlanData> allPlans = from developer in developers
                                             from plan in developer.Plans
                                             select plan;

            CalculateCapacity(days, config.Developers);
            CalculateTotalAndPromised(days, allPlans);

            int startWeek = GetWeeknum(days.First().Date);
            int weekCount = Helpers.GetWeeksBetweenDates(days.First().Date, days.Last().Date);
            List<Week> weeks = new(from weeknum in Enumerable.Range(startWeek, weekCount)
                                   select new Week(weeknum));

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

            return new(config.Settings.Fehlerteam, config.Settings.Burndown, config.Settings.Entwicklungsstart.Value, config.Settings.Entwicklungsschluss.Value, weeks, developers);
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

        private static void CalculateTotalAndPromised(List<Day> days, IEnumerable<PlanData> plans)
        {
            foreach (Day day in days)
            {
                foreach (PlanData plan in plans)
                {
                    double remaining = GetRemainingAtDate(day.Date, plan);
                    day.Total += remaining;
                    if (plan.Promised)
                    {
                        day.Promised += remaining;
                    }
                }
            }
        }

        private static double GetRemainingAtDate(DateTime date, PlanData plan)
        {
            if (date <= plan.RemainingPerDay.First().Key.Date)
            {
                return plan.RemainingPerDay.First().Value;
            }

            if (date > plan.RemainingPerDay.Last().Key.Date)
            {
                return 0.0;
            }

            return Math.Max(0, plan.RemainingPerDay.FirstOrDefault((kvp) => date == kvp.Key.Date).Value);
        }

        private static DeveloperData PrepareDeveloper(IEnumerable<Day> days, Developer developer)
        {
            IEnumerable<Day> freeDays = from day in days
                                        where !developer.IsRegularWorkDay(day.Date)
                                        select day;

            IEnumerable<PlanData> plans = PreparePlans(days, developer);

            IEnumerable<VacationData> vacations = from vacation in developer.Vacations
                                                  where vacation.Start is not null
                                                  where vacation.End is not null
                                                  select PrepareVacation(days, vacation);

            return new(developer.Abbreviation, freeDays, plans, vacations);
        }

        private static List<PlanData> PreparePlans(IEnumerable<Day> days, Developer developer)
        {
            if (developer.Plans.Count == 0)
            {
                return [];
            }

            double dailyPT = developer.GetDailyPT();
            List<PlanData> data = [];
            int currentPlanIdx = 0;
            Day? currentStart = null;
            var dayEnumerator = days.GetEnumerator();
            Dictionary<Day, double> remainingPerDay = [];
            double remainingPT = 0;
            while (currentPlanIdx < developer.Plans.Count)
            {
                if (!dayEnumerator.MoveNext())
                {
                    break;
                }

                if (!developer.IsWorkDay(dayEnumerator.Current.Date))
                {
                    remainingPerDay.Add(dayEnumerator.Current, remainingPT);
                    continue;
                }

                if (currentStart is null)
                {
                    currentStart = dayEnumerator.Current;
                    remainingPT += developer.Plans[currentPlanIdx].GetTotalPT();
                    remainingPerDay = [];
                }

                remainingPT -= dailyPT;
                if (remainingPT > 0.01)
                {
                    remainingPerDay.Add(dayEnumerator.Current, remainingPT);
                    continue;
                }

                remainingPerDay.Add(dayEnumerator.Current, remainingPT);
                data.Add(PreparePlan(developer.Plans[currentPlanIdx], remainingPerDay));
                remainingPerDay = [];
                currentPlanIdx++;
                currentStart = null;
            }

            return data;
        }

        private static PlanData PreparePlan(Plan plan, Dictionary<Day, double> remainingPerDay)
        {
            Ticket? ticket = (plan as TicketPlan)?.Ticket;

            return new(
                GetPlanType(plan),
                remainingPerDay,
                (plan as TicketPlan)?.TimeEstimateOverride?.RemainingEstimate ?? ticket?.RemainingEstimate,
                ticket?.Key ?? (plan as SpecialPlan)?.Description ?? "",
                ticket?.Summary ?? "",
                (plan as TicketPlan)?.Description ?? "",
                ticket is null || ticket.Promised);
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
            double dailyPT = vacation.Developer!.GetDailyPT();

            Dictionary<Day, double> vacationDays = new(from day in days
                                                       where vacation.IsInside(day.Date)
                                                       select new KeyValuePair<Day, double>(day, vacation.Developer!.IsRegularWorkDay(day.Date) ? dailyPT : 0.0));
            return new(vacationDays, vacation.Label);
        }
    }
}