using TrimesterPlaner.Extensions;
using TrimesterPlaner.Models;
using TrimesterPlaner.Services;

namespace TrimesterPlaner.Providers
{
    public class ConfigProvider : IValueProvider<Config>
    {
        public Config Get()
        {
            return new Config
            {
                Settings = Inject.GetValue<Settings>(),
                Developers = [.. Inject.GetCollection<Developer>()],
                Vacations = [.. Inject.GetCollection<Vacation>()],
                Tickets = [.. Inject.GetCollection<Ticket>()],
                Plans = [.. Inject.GetCollection<Plan>()],
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
                Inject.SetValue(config.Settings);
            }
            Inject.SetCollection(config.Developers);
            Inject.SetCollection(config.Vacations);
            Inject.SetCollection(config.Tickets);
            Inject.SetCollection(config.Plans);

            Inject.Require<IPlaner>().RefreshPlan();
        }
    }
}