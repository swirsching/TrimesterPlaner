using System.Collections.ObjectModel;
using TrimesterPlaner.Extensions;
using TrimesterPlaner.Models;
using TrimesterPlaner.Services;

namespace TrimesterPlaner.Providers
{
    public interface IDeveloperProvider : ICollectionProvider<Developer>
    {
        public Developer AddDeveloper(string name);
    }

    public class DeveloperProvider(
        IVacationProvider vacationProvider,
        IPlanProvider planProvider,
        IPlaner planer) : IDeveloperProvider
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
            vacationProvider.RemoveVacations(developer);
            planProvider.RemovePlans(developer);

            Developers.Remove(developer);
            planer.RefreshPlan();
        }

        public Developer AddDeveloper(string name)
        {
            Developer developer = new()
            {
                Name = name,
                Abbreviation = name[0..3].ToUpper(),
            };
            Developers.Add(developer);
            planer.RefreshPlan();
            return developer;
        }
    }
}