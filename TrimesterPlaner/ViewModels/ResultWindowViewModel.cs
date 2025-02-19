using TrimesterPlaner.Utilities;

namespace TrimesterPlaner.ViewModels
{
    public class ResultWindowViewModel(IEntwicklungsplanManager entwicklungsplanManager) : BindableBase()
    {
        public string Title { get; } = entwicklungsplanManager.GetSettings().Title;
    }
}