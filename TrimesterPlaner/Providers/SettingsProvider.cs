using TrimesterPlaner.Models;

namespace TrimesterPlaner.Providers
{
    public delegate void SettingsChangedHandler(Settings settings);
    public interface ISettingsProvider : IValueProvider<Settings>
    {
        public event SettingsChangedHandler? SettingsChanged;
        public void InvokeSettingsChanged();
    }

    public class SettingsProvider : ISettingsProvider
    {
        public event SettingsChangedHandler? SettingsChanged;
        public void InvokeSettingsChanged()
        {
            SettingsChanged?.Invoke(Settings);
        }

        private Settings Settings { get; set; } = new();

        public Settings Get()
        {
            return Settings;
        }

        public void Set(Settings? settings)
        {
            if (settings is null)
            {
                return;
            }
            Settings = settings;
            InvokeSettingsChanged();
        }
    }
}