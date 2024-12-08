using TrimesterPlaner.Models;

namespace TrimesterPlaner.ViewModels
{
    public class SettingsViewModel(Settings settings, IEntwicklungsplanManager entwicklungsplanManager) : BaseViewModel(entwicklungsplanManager)
    {
        public DateTime? Start
        {
            get => settings.Start;
            set
            {
                settings.Start = value;
                OnPropertyChanged();
            }
        }

        public DateTime? Entwicklungsstart
        {
            get => settings.Entwicklungsstart;
            set
            {
                settings.Entwicklungsstart = value;
                OnPropertyChanged();
            }
        }

        public DateTime? Entwicklungsschluss
        {
            get => settings.Entwicklungsschluss;
            set
            {
                settings.Entwicklungsschluss = value;
                OnPropertyChanged();
            }
        }

        public string Title
        {
            get => settings.Title;
            set
            {
                settings.Title = value;
                OnPropertyChanged();
            }
        }

        public string JQL
        {
            get => settings.JQL;
            set
            {
                settings.JQL = value;
                OnPropertyChanged();
            }
        }

        public bool Burndown
        {
            get => settings.Burndown;
            set
            {
                settings.Burndown = value;
                OnPropertyChanged();
            }
        }
    }
}