using TrimesterPlaner.Services;
using TrimesterPlaner.Utilities;

namespace TrimesterPlaner.ViewModels
{
    public abstract class BaseViewModel(IPlaner? trimesterPlaner) : BindableBase
    {
        protected override void OnAfterPropertyChanged(string? propertyName)
        {
            trimesterPlaner?.RefreshPlan();
        }
    }
}