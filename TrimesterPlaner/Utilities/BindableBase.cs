using System.Runtime.CompilerServices;
using TrimesterPlaner.Services;
using Utilities.Extensions;
using Utilities.Utilities;

namespace TrimesterPlaner.Utilities
{
    public abstract class BindableBase : PropertyChangedBase
    {
        protected override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            Inject.Require<IPlaner>().RefreshPlan();
        }
    }
}