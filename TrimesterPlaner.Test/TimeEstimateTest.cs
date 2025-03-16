using TrimesterPlaner.Extensions;
using TrimesterPlaner.Models;

namespace TrimesterPlaner.Test
{
    [TestClass]
    public class TimeEstimateTest
    {
        [TestMethod]
        // If there is no TimeSpent, the bigger of OriginalEstimate or RemainingEstimate should determine the result
        public void TestNoTimeSpent()
        {
            Assert.AreEqual(3, new TimeEstimate { OriginalEstimate = 3, RemainingEstimate = 3, TimeSpent = 0 }.GetTotalPT());
            Assert.AreEqual(5, new TimeEstimate { OriginalEstimate = 3, RemainingEstimate = 5, TimeSpent = 0 }.GetTotalPT());
            Assert.AreEqual(3, new TimeEstimate { OriginalEstimate = 3, RemainingEstimate = 2, TimeSpent = 0 }.GetTotalPT());
            Assert.AreEqual(3, new TimeEstimate { OriginalEstimate = 3, RemainingEstimate = 0, TimeSpent = 0 }.GetTotalPT());
        }

        [TestMethod]
        // If there is a TimeSpent, the result should be RemainingEstimate plus TimeSpent
        public void TestWithTimeSpent()
        {
            Assert.AreEqual(5, new TimeEstimate { OriginalEstimate = 3, RemainingEstimate = 0, TimeSpent = 5 }.GetTotalPT());
            Assert.AreEqual(2, new TimeEstimate { OriginalEstimate = 3, RemainingEstimate = 0, TimeSpent = 2 }.GetTotalPT());
            Assert.AreEqual(3, new TimeEstimate { OriginalEstimate = 3, RemainingEstimate = 1, TimeSpent = 2 }.GetTotalPT());
            Assert.AreEqual(2, new TimeEstimate { OriginalEstimate = 3, RemainingEstimate = 1, TimeSpent = 1 }.GetTotalPT());
        }

        [TestMethod]
        public void TestZeroEstimate()
        {
            Assert.AreEqual(0, new TimeEstimate().GetTotalPT());
        }

        [TestMethod]
        public void TestShirtSize()
        {
            ShirtSize shirt = ShirtSize.S;
            Assert.AreEqual(shirt.ToPT(), new Ticket() { Shirt = shirt }.GetTotalPT());
        }
    }
}