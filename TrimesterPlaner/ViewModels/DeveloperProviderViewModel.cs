using System.Windows.Input;
using TrimesterPlaner.Models;
using TrimesterPlaner.Providers;
using TrimesterPlaner.Utilities;
using Utilities.Extensions;
using Utilities.Utilities;

namespace TrimesterPlaner.ViewModels
{
    public class DeveloperProviderViewModel : BindableBase
    {
        public DeveloperProviderViewModel()
        {
            AddDeveloperCommand = new RelayCommand((o) => SelectedDeveloper = Inject.Require<IDeveloperProvider>().AddDeveloper("Neuling"));
            RemoveDeveloperCommand = new RelayCommand((o) => Inject.Require<IDeveloperProvider>().Remove(SelectedDeveloper!));
        }

        public IEnumerable<Developer> Developers { get; } = Inject.Require<IDeveloperProvider>().Get();

        private Developer? _SelectedDeveloper;
        public Developer? SelectedDeveloper
        {
            get => _SelectedDeveloper;
            set { if (SetProperty(ref _SelectedDeveloper, value)) Inject.Require<IDeveloperProvider>().SelectDeveloper(value); }
        }

        public ICommand AddDeveloperCommand { get; }
        public ICommand RemoveDeveloperCommand { get; }
    }
}