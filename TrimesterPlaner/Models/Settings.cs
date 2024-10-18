namespace TrimesterPlaner.Models
{
    public class Settings
    {
        public Settings() 
        {
            Start = null;
            Entwicklungsstart = null;
            Entwicklungsschluss = null;
            JQL = "";
            FehlerPT = 0;
            Fehlerteam = true;
            Burndown = true;
        }

        public DateTime? Start { get; set; }
        public DateTime? Entwicklungsstart { get; set; }
        public DateTime? Entwicklungsschluss { get; set; }
        public string JQL { get; set; }
        public double FehlerPT { get; set; }
        public bool Fehlerteam { get; set; }
        public bool Burndown { get; set; }
    }
}