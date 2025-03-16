using TrimesterPlaner.Extensions;
using TrimesterPlaner.Models;
using TrimesterPlaner.Providers;
using TrimesterPlaner.Utilities;

namespace TrimesterPlaner.ViewModels
{
    public class ResultWindowViewModel : BindableBase
    {
        public string Title { get; } = Inject.GetValue<Settings>().Title;
    }
}