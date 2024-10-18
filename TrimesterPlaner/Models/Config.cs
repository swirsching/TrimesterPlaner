using System.Collections.Immutable;

namespace TrimesterPlaner.Models
{
    public class Config
    {
        public Config()
        {
            Settings = null;
            Developers = [];
            Vacations = [];
            Tickets = [];
            Plans = [];
        }

        public Settings? Settings { get; set; }
        public ImmutableArray<Developer> Developers { get; set; }
        public ImmutableArray<Vacation> Vacations { get; set; }
        public ImmutableArray<Ticket> Tickets { get; set; }
        public ImmutableArray<Plan> Plans { get; set; }
    }
}