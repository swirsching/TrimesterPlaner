using TrimesterPlaner.Providers;
using TrimesterPlaner.Utilities;
using Utilities.Extensions;

namespace TrimesterPlaner.ViewModels
{
    public class ResultWindowViewModel : BindableBase
    {
        public string Title { get; } = Inject.Require<ISettingsProvider>().Get().Title;
    }
}