using TrimesterPlaner.Models;

namespace TrimesterPlaner.Providers
{
    public interface ISettingsProvider
    {
        public void Set(Settings settings);
        public Settings Get();
    }

    public class SettingsProvider : ISettingsProvider
    {
        private Settings Settings { get; set; } = new();

        public Settings Get()
        {
            return Settings;
        }

        public void Set(Settings settings)
        {
            Settings = settings;
        }
    }
}