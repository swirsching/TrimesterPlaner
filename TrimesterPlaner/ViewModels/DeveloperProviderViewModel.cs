using System.Windows.Input;
using TrimesterPlaner.Models;
using TrimesterPlaner.Utilities;

namespace TrimesterPlaner.ViewModels
{
    public interface IDeveloperProvider
    {
        public IEnumerable<Developer> GetDevelopers();
    }

    public class DeveloperProviderViewModel : BindableBase
    {
        public DeveloperProviderViewModel(IDeveloperManager developerManager, IDeveloperProvider developerProvider)
        {
            Developers = developerProvider.GetDevelopers();

            AddDeveloperCommand = new RelayCommand((o) => SelectedDeveloper = developerManager.AddDeveloper("Neuling"));
            RemoveDeveloperCommand = new RelayCommand((o) => developerManager.RemoveDeveloper(SelectedDeveloper!));
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