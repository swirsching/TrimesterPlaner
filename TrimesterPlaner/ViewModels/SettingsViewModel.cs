using TrimesterPlaner.Models;
using TrimesterPlaner.Providers;
using TrimesterPlaner.Services;
using TrimesterPlaner.Utilities;
using Utilities.Extensions;

namespace TrimesterPlaner.ViewModels
{
    public class SettingsViewModel : BindableBase
    {
        public SettingsViewModel()
        {
            Inject.Require<ISettingsProvider>().SettingsChanged += (settings) =>
            {
                Settings = settings;
                foreach (var property in typeof(SettingsViewModel).GetProperties())
                {
                    OnPropertyChanged(property.Name);
                }
            };
        }

        public bool HasCAT { get; } = Inject.Require<IConfluenceClient>().HasCAT();
        private Settings Settings { get; set; } = Inject.Require<ISettingsProvider>().Get();

        public DateTime? Start
        {
            get => Settings.Start;
            set
            {
                Settings.Start = value;
                OnPropertyChanged();
                Inject.Require<ISettingsProvider>().InvokeSettingsChanged();
            }
        }

        public DateTime? Entwicklungsstart
        {
            get => Settings.Entwicklungsstart;
            set
            {
                Settings.Entwicklungsstart = value;
                OnPropertyChanged();
                Inject.Require<ISettingsProvider>().InvokeSettingsChanged();
            }
        }

        public DateTime? Entwicklungsschluss
        {
            get => Settings.Entwicklungsschluss;
            set
            {
                Settings.Entwicklungsschluss = value;
                OnPropertyChanged();
                Inject.Require<ISettingsProvider>().InvokeSettingsChanged();
            }
        }

        public string Title
        {
            get => Settings.Title;
            set
            {
                Settings.Title = value;
                OnPropertyChanged();
                Inject.Require<ISettingsProvider>().InvokeSettingsChanged();
            }
        }

        public string JQL
        {
            get => Settings.JQL;
            set
            {
                Settings.JQL = value;
                OnPropertyChanged();
                Inject.Require<ISettingsProvider>().InvokeSettingsChanged();
            }
        }

        public bool Burndown
        {
            get => Settings.Burndown;
            set
            {
                Settings.Burndown = value;
                OnPropertyChanged();
                Inject.Require<ISettingsProvider>().InvokeSettingsChanged();
            }
        }

        public int PageID
        {
            get => Settings.PageID;
            set
            {
                Settings.PageID = value;
                OnPropertyChanged();
                Inject.Require<ISettingsProvider>().InvokeSettingsChanged();
            }
        }
    }
}