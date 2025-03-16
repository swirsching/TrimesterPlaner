using TrimesterPlaner.Models;

namespace TrimesterPlaner.Providers
{
    public class SettingsProvider : IValueProvider<Settings>
    {
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
        }
    }
}