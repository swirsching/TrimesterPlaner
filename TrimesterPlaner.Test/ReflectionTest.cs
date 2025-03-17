using TrimesterPlaner.Extensions;
using TrimesterPlaner.Models;

namespace TrimesterPlaner.Test
{
    [TestClass]
    public class ReflectionTest
    {
        [TestMethod]
        public void TestUpdateTicket()
        {
            var ticket1 = new Ticket();
            var ticket2 = new Ticket
            {
                Key = "12345",
                Summary = "Summary",
                Shirt = ShirtSize.S,
                Rank = "Rank",
                IsInJQL = true,
                OriginalEstimate = 3,
                RemainingEstimate = 3,
                TimeSpent = 3,
            };

            Assert.AreNotEqual(ticket1.Key, ticket2.Key);
            Assert.AreNotEqual(ticket1.Summary, ticket2.Summary);
            Assert.AreNotEqual(ticket1.Shirt, ticket2.Shirt);
            Assert.AreNotEqual(ticket1.Rank, ticket2.Rank);
            Assert.AreNotEqual(ticket1.IsInJQL, ticket2.IsInJQL);
            Assert.AreNotEqual(ticket1.OriginalEstimate, ticket2.OriginalEstimate);
            Assert.AreNotEqual(ticket1.RemainingEstimate, ticket2.RemainingEstimate);
            Assert.AreNotEqual(ticket1.TimeSpent, ticket2.TimeSpent);
            ticket1.Update(ticket2);
            Assert.AreEqual(ticket1.Key, ticket2.Key);
            Assert.AreEqual(ticket1.Summary, ticket2.Summary);
            Assert.AreEqual(ticket1.Shirt, ticket2.Shirt);
            Assert.AreEqual(ticket1.Rank, ticket2.Rank);
            Assert.AreEqual(ticket1.IsInJQL, ticket2.IsInJQL);
            Assert.AreEqual(ticket1.OriginalEstimate, ticket2.OriginalEstimate);
            Assert.AreEqual(ticket1.RemainingEstimate, ticket2.RemainingEstimate);
            Assert.AreEqual(ticket1.TimeSpent, ticket2.TimeSpent);
        }
    }
}
