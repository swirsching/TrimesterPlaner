using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace TrimesterPlaner.Models
{
    public class TicketByKeyComparer : IEqualityComparer<Ticket>
    {
        public bool Equals(Ticket? x, Ticket? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null || y is null) return false;
            return x.Key.Equals(y.Key);
        }

        public int GetHashCode([DisallowNull] Ticket obj)
        {
            return obj.Key.GetHashCode();
        }
    }

    public enum ShirtSize { Mini, XXS, XS, S, M, L, XL, XXL }
    public class Ticket : TimeEstimate
    {
        public Ticket()
        {
            Key = "";
            Summary = "";
            Shirt = null;
            Rank = "";
            IsInJQL = false;

            Plans = [];
        }

        public string Key { get; set; }
        public string Summary { get; set; }
        public ShirtSize? Shirt { get; set; }
        public string Rank { get; set; }
        public bool IsInJQL { get; set; }

        [JsonIgnore]
        public List<TicketPlan> Plans { get; }
    }
}