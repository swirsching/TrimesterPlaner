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
        public void TestPlanPreparationWithoutPlans()
        {
            List<PlanData> plans = Preparator.PreparePlans([]);
            Assert.AreEqual(0, plans.Count);
        }

        [TestMethod]
        public void TestPlanPreparationOnePlan()
        {
            List<PlanData> plans = Preparator.PreparePlans([new BugPlan() { PT = 3 }]);
            Assert.AreEqual(1, plans.Count);
            Assert.AreEqual(0, plans[0].StartPT);
            Assert.AreEqual(3, plans[0].EndPT);
            Assert.AreEqual(0, plans[0].RemainingPT);
        }

        [TestMethod]
        public void TestPlanPreparationTwoPlans()
        {
            Ticket ticket = new() { OriginalEstimate = 3.2, RemainingEstimate = 3 };
            List<PlanData> plans = Preparator.PreparePlans([new TicketPlan() { Ticket = ticket }, new BugPlan() { PT = 5.3 }]);
            Assert.AreEqual(2, plans.Count);

            Assert.AreEqual(0, plans[0].StartPT);
            Assert.AreEqual(3.2, plans[0].EndPT);
            Assert.AreEqual(0.2, plans[0].RemainingPT, 0.001);
            
            Assert.AreEqual(3.2, plans[1].StartPT);
            Assert.AreEqual(8.5, plans[1].EndPT);
            Assert.AreEqual(3.2, plans[1].RemainingPT);
        }

        [TestMethod]
        public void TestPlanPreparationMetadata()
        {
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

            List<PlanData> plans = Preparator.PreparePlans([ticketPlan]);
            Assert.AreEqual(1, plans.Count);

            var plan = plans[0];
            Assert.AreEqual(PlanType.Ticket, plan.PlanType);
            Assert.AreEqual(ticket.Key, plan.FirstRow);
            Assert.AreEqual(ticket.Summary, plan.SecondRow);
            Assert.AreEqual(ticketPlan.Description, plan.TopLeft);
        }
    }
}