using Statistics.Models;
using Utilities.Extensions;
using Utilities.Providers;

namespace Statistics.Providers
{
    public interface IConfigProvider : IValueProvider<Config>
    { }

    public class ConfigProvider : IConfigProvider
    {
        public Config Get()
        {
            return new Config
            {
                Settings = Inject.Require<ISettingsProvider>().Get(),
                Statistics = [.. Inject.Require<IStatisticsProvider>().Get()],
            };
        }

        public void Set(Config? value)
        {
            if (value is null)
            {
                return;
            }

            Inject.Require<ISettingsProvider>().Set(value.Settings);
            Inject.Require<IStatisticsProvider>().Set(value.Statistics);
        }
    }
}