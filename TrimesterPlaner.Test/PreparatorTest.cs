using System.Globalization;
using System.Reflection;
using System.Windows.Navigation;
using TrimesterPlaner.Extensions;
using TrimesterPlaner.Models;

namespace TrimesterPlaner.Test
{
    [TestClass]
    public class PreparatorTest
    {
        private List<Day> Days { get; } = new(from date in Helpers.GetDaysBetweenDates(new DateTime(2024, 11, 4), new DateTime(2024, 12, 20)) select new Day(date, false));

        [TestMethod]
        public void TestPlanPreparationWithoutPlans()
        {
            List<PlanData> plans = Preparator.PreparePlans(Days, new Developer());
            Assert.AreEqual(0, plans.Count);
        }

        [TestMethod]
        public void TestPlanPreparationMetadata()
        {
            Developer developer = new();

            Ticket ticket = new() 
            {
                Key = "E20-12345", 
                Summary = "Test-Ticket",
            };
            
            var ticketPlan = new TicketPlan()
            {
                Ticket = ticket,
                Developer = developer,
                Description = "Beschreibung",
            };

            List<PlanData> plans = Preparator.PreparePlans(Days, developer);
            Assert.AreEqual(1, plans.Count);

            var plan = plans[0];
            Assert.AreEqual(PlanType.Ticket, plan.PlanType);
            Assert.AreEqual(ticket.Key, plan.FirstRow);
            Assert.AreEqual(ticket.Summary, plan.SecondRow);
            Assert.AreEqual(ticketPlan.Description, plan.TopLeft);
        }

        [TestMethod]
        [DynamicData(nameof(PlanData), DynamicDataDisplayName = nameof(PlanDataDisplayName))]
        public void TestPlanPreparation(Developer developer, List<double> PTs, List<string> expectedRemainingsPerDay)
        {
            Assert.AreEqual(PTs.Count, expectedRemainingsPerDay.Count);

            foreach (var pt in PTs)
            {
                _ = new BugPlan()
                {
                    PT = pt,
                    Developer = developer,
                };
            }

            List<PlanData> plans = Preparator.PreparePlans(Days, developer);
            Assert.AreEqual(PTs.Count, plans.Count);

            foreach (var (plan, expectedRemainings) in plans.Zip(expectedRemainingsPerDay))
            {
                var remainingsPerDay = from remainingPerDay in plan.RemainingPerDay select Math.Round(remainingPerDay.Value, 2).ToString(new NumberFormatInfo() { NumberDecimalSeparator = "." });
                Assert.AreEqual(expectedRemainings, string.Join(" ", remainingsPerDay));
            }
        }

        public static string PlanDataDisplayName(MethodInfo methodInfo, object[] data)
        {
            var developer = (Developer)data[0];
            var PTs = (List<double>)data[1];
            return $"{methodInfo.Name} - [{developer.GetDailyPT().ToString(new NumberFormatInfo() { NumberDecimalSeparator = "." })} PT pro Tag, {developer.FreeDays.Count} freie Tage, {string.Join("+", PTs)} PT]";
        }

        public static IEnumerable<object[]> PlanData
        {
            get =>
            [
                // Tests with one plan:
                [new Developer() { Verwaltung = 0, FreeDays = [] }, new List<double>() { 14 }, new List<string>() { "13 12 11 10 9 8 7 6 5 4 3 2 1 0" }],
                [new Developer() { Verwaltung = 0 }, new List<double>() { 10 }, new List<string>() { "9 8 7 6 5 5 5 4 3 2 1 0" } ],
                [new Developer(), new List<double>() { 3 }, new List<string>() { "2.1 1.2 0.3 -0.6" } ],
                [new Developer() { FTE = 80, Sonderrolle = 20 }, new List<double>() { 4 }, new List<string>() { "3.44 2.88 2.32 1.76 1.2 1.2 1.2 0.64 0.08 -0.48" } ],

                // Tests with two plans:
                [new Developer() { FreeDays = [] }, new List<double>() { 3, 5 }, new List<string>() { "2.1 1.2 0.3 -0.6", "3.5 2.6 1.7 0.8 -0.1" }],
                [new Developer() { FreeDays = [] }, new List<double>() { 5, 3 }, new List<string>() { "4.1 3.2 2.3 1.4 0.5 -0.4", "1.7 0.8 -0.1" }],
            ];
        }
    }
}