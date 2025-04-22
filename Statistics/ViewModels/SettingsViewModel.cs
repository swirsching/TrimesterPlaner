using Statistics.Models;
using Statistics.Providers;
using Utilities.Extensions;
using Utilities.Utilities;

namespace Statistics.ViewModels
{
    public class SettingsViewModel : PropertyChangedBase
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

        private Settings Settings { get; set; } = Inject.Require<ISettingsProvider>().Get();

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

        public int UpdateFrequencyInMinutes
        {
            get => Settings.UpdateFrequencyInMinutes;
            set
            {
                Settings.UpdateFrequencyInMinutes = value;
                OnPropertyChanged();
                Inject.Require<ISettingsProvider>().InvokeSettingsChanged();
            }
        }
    }
}