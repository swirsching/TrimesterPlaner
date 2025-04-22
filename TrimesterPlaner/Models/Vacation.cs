namespace TrimesterPlaner.Models
{
    public class Vacation
    {
        private Developer? _Developer = null;
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

        public DateTime? Start { get; set; } = null;
        public DateTime? End { get; set; } = null;
        public string Label { get; set; } = "";
    }
}