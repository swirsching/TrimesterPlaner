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