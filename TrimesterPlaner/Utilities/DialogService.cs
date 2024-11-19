using TrimesterPlaner.ViewModels;
using TrimesterPlaner.Views;

namespace TrimesterPlaner.Utilities
{
    public class DialogService
    {
        public static bool? ShowEmptyDialog(string title)
        {
            Dialog dialog = new() { DataContext = new DialogViewModel(title) };
            return dialog.ShowDialog();
        }
    }
}