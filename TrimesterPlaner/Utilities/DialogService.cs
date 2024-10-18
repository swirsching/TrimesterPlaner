using TrimesterPlaner.ViewModels;
using TrimesterPlaner.Views;

namespace TrimesterPlaner.Utilities
{
    public class DialogService
    {
        public static T? AskUser<T>(T? initial = default)
        {
            DialogViewModel viewModel = new();
            viewModel.Initialize(initial);
            Dialog dialog = new() { DataContext = viewModel };

            if (dialog.ShowDialog() == true)
            {
                return viewModel.GetResult<T>();
            }
            return default;
        }
    }
}