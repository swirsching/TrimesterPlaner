using Microsoft.Extensions.DependencyInjection;
using TrimesterPlaner.Extensions;
using TrimesterPlaner.Models;
using TrimesterPlaner.ViewModels;

namespace TrimesterPlaner.Providers
{
    public interface IConfigProvider : IValueProvider<Config>
    {
    }

    public class ConfigProvider(
        ISettingsProvider settingsProvider,
        IDeveloperProvider developerProvider,
        IVacationProvider vacationProvider,
        ITicketProvider ticketProvider,
        IPlanProvider planProvider,
        IEntwicklungsplanManager entwicklungsplanManager) : IConfigProvider
    {
        public Config Get()
        {
            return new Config
            {
                Settings = settingsProvider.Get(),
                Developers = [.. developerProvider.Get()],
                Vacations = [.. vacationProvider.Get()],
                Tickets = [.. ticketProvider.Get()],
                Plans = [.. planProvider.Get()],
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
                settingsProvider.Set(config.Settings);
            }
            developerProvider.Set(config.Developers);
            vacationProvider.Set(config.Vacations);
            ticketProvider.Set(config.Tickets);
            planProvider.Set(config.Plans);

            entwicklungsplanManager.RefreshEntwicklungsplan();
        }
    }
}