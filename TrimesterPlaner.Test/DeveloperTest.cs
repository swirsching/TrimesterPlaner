using TrimesterPlaner.Extensions;
using TrimesterPlaner.Models;

namespace TrimesterPlaner.Test
{
    [TestClass]
    public class DeveloperTest
    {
        [TestMethod]
        [DataRow(1, 100, 0, 0)]
        [DataRow(0.9, 100, 0, 10)]
        [DataRow(0.9, 100, 10, 0)]
        [DataRow(0.6, 100, 30, 10)]
        [DataRow(0.8, 80, 0, 0)]
        [DataRow(0.64, 80, 20, 0)]
        public void TestDailyPT(double expectedPT, int fte, int sonderrolle, int verwaltung)
        {
            Developer developer = new()
            { 
                FTE = fte,
                Sonderrolle = sonderrolle,
                Verwaltung = verwaltung,
            };

            Assert.AreEqual(expectedPT, Math.Round(developer.GetDailyPT(), 2));
        }
    }
}