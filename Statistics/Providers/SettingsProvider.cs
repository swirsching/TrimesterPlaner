using Statistics.Models;
using Utilities.Providers;

namespace Statistics.Providers
{
    public interface ISettingsProvider : ISettingsProviderBase<Settings>
    { }

    public class SettingsProvider : SettingsProviderBase<Settings>, ISettingsProvider
    { }
}
