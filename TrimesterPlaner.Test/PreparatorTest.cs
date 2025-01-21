using TrimesterPlaner.Extensions;
using TrimesterPlaner.Models;

namespace TrimesterPlaner.Test
{
    [TestClass]
    public class PreparatorTest
    {
        [TestMethod]
        [DataRow(0, 1, 0, 0)]
        [DataRow(0, 1, 1, 1)]
        [DataRow(0, 1, 0.5, 0.5)]
        [DataRow(0.5, 1, 0.5, 0)]
        [DataRow(2, 3.5, 2.5, 0.33)]
        public void TestAlpha(double before, double after, double pt, double expected)
        {
            var alpha = PreparedDataExtensions.GetAlpha(before, after, pt);
            Assert.AreEqual(expected, alpha, 0.01);
        }

        [TestMethod]
        public void TestDayPreparation()
        {
            IEnumerable<Day> days = Preparator.PrepareDays(new DateTime(2024, 10, 1), new DateTime(2025, 10, 1), new DateTime(2024, 12, 10));
            Assert.AreEqual(366, days.Count());
            Assert.AreEqual(Widths.Left + Widths.WeekDay * 5 + Widths.WeekEndDay * 2, days.ElementAt(7).GetX(0));
            Assert.AreEqual(Widths.Left + Widths.WeekDay * 14 + Widths.WeekEndDay * 4, days.ElementAt(18).GetX(0));
        }

        [TestMethod]
        public void TestDayPreparationForDeveloper()
        {
            IEnumerable<Day> days = Preparator.PrepareDays(new DateTime(2024, 10, 1), new DateTime(2025, 10, 1), new DateTime(2024, 12, 10));
            Developer developer = new();
            IEnumerable<DayWithPT> daysWithPT = Preparator.PrepareDaysForDeveloper(days, developer);

            Assert.AreEqual(days.Count(), daysWithPT.Count());

            var firstDay = daysWithPT.First();
            var dailyPT = developer.GetDailyPT();
            Assert.AreEqual(0, firstDay.Before);
            Assert.AreEqual(dailyPT, firstDay.After);

            foreach (var (a, b) in daysWithPT.Take(daysWithPT.Count() - 1).Zip(daysWithPT.Skip(1)))
            {
                Assert.AreEqual(a.After, b.Before);
            }

            Assert.AreEqual(262 * dailyPT, daysWithPT.Last().After, 0.01);
        }

        [TestMethod]
        public void TestPlanPreparationWithoutPlans()
        {
            IEnumerable<Day> days = Preparator.PrepareDays(new DateTime(2024, 10, 1), new DateTime(2025, 10, 1), new DateTime(2024, 12, 10));
            IEnumerable<DayWithPT> daysWithPT = Preparator.PrepareDaysForDeveloper(days, new Developer());

            List<PlanData> plans = Preparator.PreparePlans(daysWithPT, []);
            Assert.AreEqual(0, plans.Count);
        }

        [TestMethod]
        public void TestPlanPreparationOnePlan()
        {
            IEnumerable<Day> days = Preparator.PrepareDays(new DateTime(2024, 10, 1), new DateTime(2025, 10, 1), new DateTime(2024, 12, 10));
            IEnumerable<DayWithPT> daysWithPT = Preparator.PrepareDaysForDeveloper(days, new Developer());

            List<PlanData> plans = Preparator.PreparePlans(daysWithPT, [new BugPlan() { PT = 3 }]);
            Assert.AreEqual(1, plans.Count);
            Assert.AreEqual(Widths.Left, plans[0].StartX);
            Assert.AreEqual(Widths.Left, plans[0].RemainingX);
            Assert.AreEqual(206, plans[0].EndX);
        }

        [TestMethod]
        public void TestPlanPreparationTwoPlans()
        {
            IEnumerable<Day> days = Preparator.PrepareDays(new DateTime(2024, 10, 1), new DateTime(2025, 10, 1), new DateTime(2024, 12, 10));
            IEnumerable<DayWithPT> daysWithPT = Preparator.PrepareDaysForDeveloper(days, new Developer());

            Ticket ticket = new() { OriginalEstimate = 3.2, RemainingEstimate = 3 };
            List<PlanData> plans = Preparator.PreparePlans(daysWithPT, [new TicketPlan() { Ticket = ticket }, new BugPlan() { PT = 5.3 }]);
            Assert.AreEqual(2, plans.Count);

            Assert.AreEqual(Widths.Left, plans[0].StartX);
            Assert.AreEqual(Widths.Left + 11, plans[0].RemainingX);
            Assert.AreEqual(217, plans[0].EndX);

            Assert.AreEqual(217, plans[1].StartX);
            Assert.AreEqual(217, plans[1].RemainingX);
            Assert.AreEqual(532, plans[1].EndX);
        }

        [TestMethod]
        public void TestPlanPreparationMetadata()
        {
            IEnumerable<Day> days = Preparator.PrepareDays(new DateTime(2024, 10, 1), new DateTime(2025, 10, 1), new DateTime(2024, 12, 10));
            IEnumerable<DayWithPT> daysWithPT = Preparator.PrepareDaysForDeveloper(days, new Developer());

            Ticket ticket = new()
            {
                Key = "E20-12345",
                Summary = "Test-Ticket",
            };

            var ticketPlan = new TicketPlan()
            {
                Ticket = ticket,
                Description = "Beschreibung",
            };

            List<PlanData> plans = Preparator.PreparePlans(daysWithPT, [ticketPlan]);
            Assert.AreEqual(1, plans.Count);

            var plan = plans[0];
            Assert.AreEqual(PlanType.Ticket, plan.PlanType);
            Assert.AreEqual(ticket.Key, plan.FirstRow);
            Assert.AreEqual(ticket.Summary, plan.SecondRow);
            Assert.AreEqual(ticketPlan.Description, plan.TopLeft);
        }

        [TestMethod]
        public void TestPlanPreparationWithEarliestStart()
        {
            IEnumerable<Day> days = Preparator.PrepareDays(new DateTime(2024, 10, 1), new DateTime(2025, 10, 1), new DateTime(2024, 12, 10));
            IEnumerable<DayWithPT> daysWithPT = Preparator.PrepareDaysForDeveloper(days, new Developer());

            List<PlanData> plans = Preparator.PreparePlans(daysWithPT, [
                new BugPlan() { PT = 3 },
                new BugPlan() { PT = 3, EarliestStart = new DateTime(2024, 10, 15) },
                new BugPlan() { PT = 3, EarliestStart = new DateTime(2024, 10, 15) },
            ]);
            Assert.AreEqual(3, plans.Count);

            Assert.AreEqual(Widths.Left, plans[0].StartX);
            Assert.AreEqual(206, plans[0].EndX);
            Assert.AreEqual(560, plans[1].StartX);
            Assert.AreEqual(726, plans[1].EndX);
            Assert.AreEqual(726, plans[2].StartX);
            Assert.AreEqual(903, plans[2].EndX);
        }
    }
}