namespace Utilities.Providers
{
    public delegate void SettingsChangedHandler<SettingsType>(SettingsType settings);
    public interface ISettingsProviderBase<SettingsType> : IValueProvider<SettingsType>
    {
        public event SettingsChangedHandler<SettingsType>? SettingsChanged;
        public void InvokeSettingsChanged();
    }

    public class SettingsProviderBase<SettingsType> : ISettingsProviderBase<SettingsType> where SettingsType : new()
    {
        public event SettingsChangedHandler<SettingsType>? SettingsChanged;
        public void InvokeSettingsChanged()
        {
            SettingsChanged?.Invoke(Settings);
        }

        private SettingsType Settings { get; set; } = new();

        public SettingsType Get()
        {
            return Settings;
        }

        public void Set(SettingsType? settings)
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