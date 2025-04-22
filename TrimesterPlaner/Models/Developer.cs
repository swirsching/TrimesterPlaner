using System.Text.Json.Serialization;

namespace TrimesterPlaner.Models
{
    [method: JsonConstructor]
    public class Developer()
    {
        public string Name { get; set; } = "";
        public string Abbreviation { get; set; } = "";
        public int FTE { get; set; } = 100;
        public int Sonderrolle { get; set; } = 0;
        public int Verwaltung { get; set; } = 10;
        public HashSet<DayOfWeek> FreeDays { get; set; } = [DayOfWeek.Saturday, DayOfWeek.Sunday];

        [JsonIgnore]
        public List<Vacation> Vacations { get; } = [];
        [JsonIgnore]
        public List<Plan> Plans { get; } = [];
    }
}