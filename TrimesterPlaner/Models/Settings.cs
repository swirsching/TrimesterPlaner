namespace TrimesterPlaner.Models
{
    public class Settings
    {
        public string Title { get; set; } = "";
        public DateTime? Start { get; set; } = null;
        public DateTime? Entwicklungsstart { get; set; } = null;
        public DateTime? Entwicklungsschluss { get; set; } = null;
        public string JQL { get; set; } = "";
        public bool Burndown { get; set; } = true;
        public int PageID { get; set; } = 0;
    }
}