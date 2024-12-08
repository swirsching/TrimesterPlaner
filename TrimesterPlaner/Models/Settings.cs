namespace TrimesterPlaner.Models
{
    public class Settings
    {
        public Settings() 
        {
            Title = "";
            Start = null;
            Entwicklungsstart = null;
            Entwicklungsschluss = null;
            JQL = "";
            Burndown = true;
        }

        public string Title { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? Entwicklungsstart { get; set; }
        public DateTime? Entwicklungsschluss { get; set; }
        public string JQL { get; set; }
        public bool Burndown { get; set; }
    }
}