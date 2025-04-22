using System.Text.Json.Serialization;

namespace TrimesterPlaner.Models
{
    [JsonDerivedType(typeof(Plan), typeDiscriminator: nameof(Plan))]
    [JsonDerivedType(typeof(TicketPlan), typeDiscriminator: nameof(TicketPlan))]
    [JsonDerivedType(typeof(BugPlan), typeDiscriminator: nameof(BugPlan))]
    [JsonDerivedType(typeof(SpecialPlan), typeDiscriminator: nameof(SpecialPlan))]
    public class Plan
    {
        private Developer? _Developer = null;
        public Developer? Developer
        {
            get => _Developer;
            set
            {
                _Developer?.Plans?.Remove(this);
                _Developer = value;
                _Developer?.Plans?.Add(this);
            }
        }

        public DateTime? EarliestStart { get; set; } = null;
    }

    public class TicketPlan : Plan
    {
        private Ticket? _Ticket = null;
        public Ticket? Ticket
        {
            get => _Ticket;
            set
            {
                _Ticket?.Plans?.Remove(this);
                _Ticket = value;
                _Ticket?.Plans?.Add(this);
            }
        }

        public TimeEstimate? TimeEstimateOverride { get; set; } = null;
        public string Description { get; set; } = "";
    }

    public class BugPlan : Plan
    {
        public double PT { get; set; } = 0;
    }

    public class SpecialPlan : Plan
    {
        public string Description { get; set; } = "";
        public int Days { get; set; } = 0;
    }
}