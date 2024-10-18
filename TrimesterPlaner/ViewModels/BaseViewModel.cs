using TrimesterPlaner.Utilities;

namespace TrimesterPlaner.ViewModels
{
    public abstract class BaseViewModel(IEntwicklungsplanManager? entwicklungsplanManager) : BindableBase
    {
        protected override void OnAfterPropertyChanged(string? propertyName)
        {
            entwicklungsplanManager?.RefreshEntwicklungsplan();
        }
    }
}