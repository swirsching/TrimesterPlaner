using TrimesterPlaner.Models;
using TrimesterPlaner.Utilities;

namespace TrimesterPlaner.ViewModels
{
    public interface IDeveloperProvider
    {
        public IEnumerable<Developer> GetDevelopers();
    }

    public class DeveloperProviderViewModel(IDeveloperProvider developerProvider) : BindableBase
    {
        public IEnumerable<Developer> Developers { get; } = developerProvider.GetDevelopers();

        private Developer? _SelectedDeveloper;
        public Developer? SelectedDeveloper
        {
            get => _SelectedDeveloper;
            set { if (SetProperty(ref _SelectedDeveloper, value)) OnSelectedDeveloperChanged?.Invoke(value); }
        }

        public delegate void OnSelectedDeveloperChangedEventHandler(Developer? selectedDeveloper);
        public event OnSelectedDeveloperChangedEventHandler? OnSelectedDeveloperChanged;
    }
}