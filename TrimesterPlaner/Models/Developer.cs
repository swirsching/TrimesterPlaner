using System.Text.Json.Serialization;

namespace TrimesterPlaner.Models
{
    public class Developer
    {
        [JsonConstructor]
        public Developer() 
        {
            Name = "";
            Abbreviation = "";
            FTE = 100;
            Sonderrolle = 0;
            Verwaltung = 10;
            FreeDays = [DayOfWeek.Saturday, DayOfWeek.Sunday];
            Vacations = [];
            Plans = [];
            Tickets = [];
        }

        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public int FTE { get; set; }
        public int Sonderrolle { get; set; }
        public int Verwaltung { get; set; }
        public HashSet<DayOfWeek> FreeDays { get; set; }

        [JsonIgnore]
        public List<Vacation> Vacations { get; }
        [JsonIgnore]
        public List<Plan> Plans { get; }
        [JsonIgnore]
        public List<Ticket> Tickets { get; }
    }
}