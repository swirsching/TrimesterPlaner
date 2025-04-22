using System.Collections.Immutable;

namespace Statistics.Models
{
    public class Config
    {
        public Settings? Settings { get; set; } = null;
        public ImmutableArray<Statistic> Statistics { get; set; } = [];
    }
}