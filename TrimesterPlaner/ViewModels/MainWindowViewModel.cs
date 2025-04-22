using TrimesterPlaner.Providers;
using TrimesterPlaner.Utilities;
using Utilities.Extensions;

namespace TrimesterPlaner.ViewModels
{
    public class MainWindowViewModel : BindableBase
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