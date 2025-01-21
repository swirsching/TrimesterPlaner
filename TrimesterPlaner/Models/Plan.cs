using System.Text.Json.Serialization;

namespace TrimesterPlaner.Models
{
    [JsonDerivedType(typeof(Plan), typeDiscriminator: nameof(Plan))]
    [JsonDerivedType(typeof(TicketPlan), typeDiscriminator: nameof(TicketPlan))]
    [JsonDerivedType(typeof(BugPlan), typeDiscriminator: nameof(BugPlan))]
    [JsonDerivedType(typeof(SpecialPlan), typeDiscriminator: nameof(SpecialPlan))]
    public class Plan
    {
        public Plan()
        {
            Developer = null;
            EarliestStart = null;
        }

        private Developer? _Developer;
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

        public DateTime? EarliestStart { get; set; }
    }

    public class TicketPlan : Plan
    {
        public TicketPlan()
        {
            Description = "";
        }

        private Ticket? _Ticket;
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

        public TimeEstimate? TimeEstimateOverride { get; set; }
        public string Description { get; set; }
    }

    public class BugPlan : Plan
    {
        public BugPlan()
        {
            PT = 0;
        }

        public double PT { get; set; }
    }

    public class SpecialPlan : Plan 
    {
        public SpecialPlan()
        {
            Description = "";
        }

        public string Description { get; set; }
        public int Days { get; set; }
    }
}