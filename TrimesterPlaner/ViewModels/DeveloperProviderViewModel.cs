using System.Windows.Input;
using TrimesterPlaner.Models;
using TrimesterPlaner.Providers;
using TrimesterPlaner.Utilities;

namespace TrimesterPlaner.ViewModels
{
    public class DeveloperProviderViewModel : BindableBase
    {
        public DeveloperProviderViewModel(IDeveloperProvider developerProvider)
        {
            Developers = developerProvider.Get();

            AddDeveloperCommand = new RelayCommand((o) => SelectedDeveloper = developerProvider.AddDeveloper("Neuling"));
            RemoveDeveloperCommand = new RelayCommand((o) => developerProvider.Remove(SelectedDeveloper!));
        }

        public IEnumerable<Developer> Developers { get; }

        private Developer? _SelectedDeveloper;
        public Developer? SelectedDeveloper
        {
            get => _SelectedDeveloper;
            set { if (SetProperty(ref _SelectedDeveloper, value)) OnSelectedDeveloperChanged?.Invoke(value); }
        }

        public delegate void OnSelectedDeveloperChangedEventHandler(Developer? selectedDeveloper);
        public event OnSelectedDeveloperChangedEventHandler? OnSelectedDeveloperChanged;

        public ICommand AddDeveloperCommand { get; }
        public ICommand RemoveDeveloperCommand { get; }
    }
}