using TrimesterPlaner.Models;

namespace TrimesterPlaner.ViewModels
{
    public interface ISettingsProvider
    {
        public Settings GetSettings();
    }

    public class SettingsViewModel(ISettingsProvider settingsProvider, IEntwicklungsplanManager entwicklungsplanManager) : BaseViewModel(entwicklungsplanManager)
    {
        private Settings Settings { get; } = settingsProvider.GetSettings();

        public DateTime? Start
        {
            get => Settings.Start;
            set
            {
                Settings.Start = value;
                OnPropertyChanged();
            }
        }

        public DateTime? Entwicklungsstart
        {
            get => Settings.Entwicklungsstart;
            set
            {
                Settings.Entwicklungsstart = value;
                OnPropertyChanged();
            }
        }

        public DateTime? Entwicklungsschluss
        {
            get => Settings.Entwicklungsschluss;
            set
            {
                Settings.Entwicklungsschluss = value;
                OnPropertyChanged();
            }
        }

        public string JQL
        {
            get => Settings.JQL;
            set
            {
                Settings.JQL = value;
                OnPropertyChanged();
            }
        }

        public double FehlerPT
        {
            get => Settings.FehlerPT;
            set
            {
                Settings.FehlerPT = value;
                OnPropertyChanged();
            }
        }

        public bool Fehlerteam
        {
            get => Settings.Fehlerteam;
            set
            {
                Settings.Fehlerteam = value;
                OnPropertyChanged();
            }
        }

        public bool Burndown
        {
            get => Settings.Burndown;
            set
            {
                Settings.Burndown = value;
                OnPropertyChanged();
            }
        }
    }
}