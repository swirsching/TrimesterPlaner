using System.Text.Json.Serialization;

namespace Statistics.Models
{
    [JsonDerivedType(typeof(Statistic), nameof(Statistic))]
    [JsonDerivedType(typeof(DateBasedStatistic), nameof(DateBasedStatistic))]
    public class Statistic
    {
        public string Header { get; set; } = "";
        public string JQL { get; set; } = "";
    }

    public enum DateField { Created, Resolved }
    public enum Grouping { Daily, Weekly, Monthly }
    public class DateBasedStatistic : Statistic
    {
        public DateField? DateField { get; set; } = null;
        public Grouping? Grouping { get; set; } = null;
    }
}