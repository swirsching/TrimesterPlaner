namespace TrimesterPlaner.Models
{
    public class Vacation
    {
        public Vacation()
        {
            Developer = null;
            Start = null;
            End = null;
            Label = "";
        }

        private Developer? _Developer;
        public Developer? Developer
        {
            get => _Developer;
            set
            {
                _Developer?.Vacations?.Remove(this);
                _Developer = value; 
                _Developer?.Vacations?.Add(this); 
            }
        }

        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public string Label { get; set; }
    }
}