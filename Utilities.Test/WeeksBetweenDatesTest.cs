using Utilities.Extensions;

namespace Utilities.Test
{
    [TestClass]
    public class WeeksBetweenDatesTest
    {
        [TestMethod]
        public void TestTuesdayToWednesday()
        {
            Assert.AreEqual(1, CalendarHelpers.GetWeeksBetweenDates(new DateTime(2024, 10, 1), new DateTime(2024, 10, 2)));
        }

        [TestMethod]
        public void TestTuesdayToWednesdayOfNextWeek()
        {
            Assert.AreEqual(2, CalendarHelpers.GetWeeksBetweenDates(new DateTime(2024, 10, 1), new DateTime(2024, 10, 9)));
        }

        [TestMethod]
        public void TestSundayToMondayOfNextWeek()
        {
            Assert.AreEqual(2, CalendarHelpers.GetWeeksBetweenDates(new DateTime(2024, 10, 6), new DateTime(2024, 10, 7)));
        }

        [TestMethod]
        public void TestSaturdayToMondayOfNextWeek()
        {
            Assert.AreEqual(2, CalendarHelpers.GetWeeksBetweenDates(new DateTime(2024, 10, 5), new DateTime(2024, 10, 7)));
        }

        [TestMethod]
        public void TestMondayToSunday()
        {
            Assert.AreEqual(1, CalendarHelpers.GetWeeksBetweenDates(new DateTime(2024, 10, 7), new DateTime(2024, 10, 13)));
        }

        [TestMethod]
        public void TestMondayToSundayOfNextWeek()
        {
            Assert.AreEqual(2, CalendarHelpers.GetWeeksBetweenDates(new DateTime(2024, 10, 7), new DateTime(2024, 10, 20)));
        }

        [TestMethod]
        public void TestThursdayToMondayOfNextWeek()
        {
            Assert.AreEqual(2, CalendarHelpers.GetWeeksBetweenDates(new DateTime(2024, 10, 3), new DateTime(2024, 10, 7)));
        }

        [TestMethod]
        public void TestYearGap()
        {
            Assert.AreEqual(1, CalendarHelpers.GetWeeksBetweenDates(new DateTime(2024, 12, 30), new DateTime(2025, 1, 1)));
        }
    }
}