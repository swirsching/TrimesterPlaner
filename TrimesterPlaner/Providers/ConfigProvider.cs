using TrimesterPlaner.Extensions;
using TrimesterPlaner.Models;
using TrimesterPlaner.Services;

namespace TrimesterPlaner.Providers
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
                Developers = [.. Inject.Require<IDeveloperProvider>().Get()],
                Vacations = [.. Inject.Require<IVacationProvider>().Get()],
                Tickets = [.. Inject.Require<ITicketProvider>().Get()],
                Plans = [.. Inject.Require<IPlanProvider>().Get()],
            };
        }

        public void Set(Config? config)
        {
            if (config is null)
            {
                return;
            }

            if (config.Settings is not null)
            {
                Inject.Require<ISettingsProvider>().Set(config.Settings);
            }
            Inject.Require<IDeveloperProvider>().Set(config.Developers);
            Inject.Require<IVacationProvider>().Set(config.Vacations);
            Inject.Require<ITicketProvider>().Set(config.Tickets);
            Inject.Require<IPlanProvider>().Set(config.Plans);

            Inject.Require<IPlaner>().RefreshPlan();
        }
    }
}