using TrimesterPlaner.Models;

namespace TrimesterPlaner.Providers
{
    public interface ISettingsProvider : IValueProvider<Settings>
    { }

    public class SettingsProvider : ISettingsProvider
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