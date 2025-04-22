using System.Windows.Input;
using TrimesterPlaner.Models;
using TrimesterPlaner.Providers;
using TrimesterPlaner.Utilities;
using Utilities.Extensions;
using Utilities.Utilities;

namespace TrimesterPlaner.ViewModels
{
    public class PlanViewModel(Plan plan) : BindableBase
    {
        public Plan Plan { get; } = plan;

        public DateTime? EarliestStart
        {
            get => Plan.EarliestStart;
            set
            {
                Plan.EarliestStart = value;
                OnPropertyChanged();
            }
        }

        private bool _IsChangingDeveloper = false;
        public bool IsChangingDeveloper
        {
            get => _IsChangingDeveloper;
            set => SetProperty(ref _IsChangingDeveloper, value);
        }

        public IEnumerable<Developer> Developers { get; } = Inject.Require<IDeveloperProvider>().Get();

        public Developer? SelectedDeveloper
        {
            get => Plan.Developer;
            set
            {
                if (value is not null)
                {
                    Plan.Developer = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand RemoveCommand { get; } = new RelayCommand((o) => Inject.Require<IPlanProvider>().Remove(plan));
    }
}