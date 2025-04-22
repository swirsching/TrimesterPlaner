using System.Collections.ObjectModel;
using TrimesterPlaner.Models;
using TrimesterPlaner.Services;
using Utilities.Extensions;

namespace TrimesterPlaner.Providers
{
    public delegate void OnSelectedDeveloperChangedEventHandler(Developer? selectedDeveloper);
    public interface IDeveloperProvider : ICollectionProvider<Developer>
    {
        public event OnSelectedDeveloperChangedEventHandler? OnSelectedDeveloperChanged;
        public void SelectDeveloper(Developer? developer);
        public Developer AddDeveloper(string name);
    }

    public class DeveloperProvider : IDeveloperProvider
    {
        private ObservableCollection<Developer> Developers { get; } = [];

        public IEnumerable<Developer> Get()
        {
            return Developers;
        }

        public void Set(IEnumerable<Developer> values)
        {
            Developers.ClearAndAdd(values, new((a, b) => a.Abbreviation.CompareTo(b.Abbreviation)));
        }

        public void Remove(Developer developer)
        {
            Inject.Require<IVacationProvider>().RemoveVacations(developer);
            Inject.Require<IPlanProvider>().RemovePlans(developer);

            Developers.Remove(developer);
            Inject.Require<IPlaner>().RefreshPlan();
        }

        public event OnSelectedDeveloperChangedEventHandler? OnSelectedDeveloperChanged;

        public void SelectDeveloper(Developer? developer)
        {
            OnSelectedDeveloperChanged?.Invoke(developer);
        }

        public Developer AddDeveloper(string name)
        {
            Developer developer = new()
            {
                Name = name,
                Abbreviation = name[0..3].ToUpper(),
            };
            Developers.Add(developer);
            Inject.Require<IPlaner>().RefreshPlan();
            return developer;
        }
    }
}