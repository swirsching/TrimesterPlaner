using System.Windows.Input;

namespace Utilities.Utilities
{
    public class RelayCommand(Action<object?> action) : ICommand
    {
#pragma warning disable CS0067 // The event is never used.
        public event EventHandler? CanExecuteChanged;
#pragma warning restore CS0067 // The event is never used.

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            action.Invoke(parameter);
        }
    }
}
