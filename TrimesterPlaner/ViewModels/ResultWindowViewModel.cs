using TrimesterPlaner.Providers;
using TrimesterPlaner.Utilities;

namespace TrimesterPlaner.ViewModels
{
    public class ResultWindowViewModel(ISettingsProvider settingsProvider) : BindableBase()
    {
        public string Title { get; } = settingsProvider.Get().Title;
    }
}