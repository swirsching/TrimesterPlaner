using TrimesterPlaner.Extensions;
using TrimesterPlaner.Models;
using TrimesterPlaner.Providers;
using TrimesterPlaner.Services;
using TrimesterPlaner.Utilities;

namespace TrimesterPlaner.ViewModels
{
    public class SettingsViewModel : BindableBase
    {
        public bool HasCAT { get; } = Inject.Require<IConfluenceClient>().HasCAT();
        private Settings Settings { get; } = Inject.Require<ISettingsProvider>().Get();

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

        public string Title
        {
            get => Settings.Title;
            set
            {
                Settings.Title = value;
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

        public bool Burndown
        {
            get => Settings.Burndown;
            set
            {
                Settings.Burndown = value;
                OnPropertyChanged();
            }
        }

        public int PageID
        {
            get => Settings.PageID;
            set
            {
                Settings.PageID = value;
                OnPropertyChanged();
            }
        }
    }
}