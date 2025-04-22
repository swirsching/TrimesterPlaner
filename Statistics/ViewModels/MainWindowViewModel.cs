using Statistics.Providers;
using Utilities.Extensions;
using Utilities.Utilities;

namespace Statistics.ViewModels
{
    public class MainWindowViewModel : PropertyChangedBase
    {
        public MainWindowViewModel()
        {
            var settingsProvider = Inject.Require<ISettingsProvider>();
            Title = settingsProvider.Get().Title;
            settingsProvider.SettingsChanged += (settings) => Title = settings.Title;
        }

        private string _Title = string.Empty;
        public string Title
        {
            get => _Title;
            set => SetProperty(ref _Title, value);
        }
    }
}