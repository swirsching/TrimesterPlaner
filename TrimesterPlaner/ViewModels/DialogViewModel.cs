using TrimesterPlaner.Utilities;

namespace TrimesterPlaner.ViewModels
{
    public class DialogViewModel(string title) : BindableBase
    {
        public string Title { get; } = title;
    }
}