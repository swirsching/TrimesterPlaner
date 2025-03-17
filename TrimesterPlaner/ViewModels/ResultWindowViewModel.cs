using TrimesterPlaner.Extensions;
using TrimesterPlaner.Providers;
using TrimesterPlaner.Utilities;

namespace TrimesterPlaner.ViewModels
{
    public class ResultWindowViewModel : BindableBase
    {
        public string Title { get; } = Inject.Require<ISettingsProvider>().Get().Title;
    }
}