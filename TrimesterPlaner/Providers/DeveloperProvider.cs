using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using TrimesterPlaner.Extensions;
using TrimesterPlaner.Models;
using TrimesterPlaner.ViewModels;

namespace TrimesterPlaner.Providers
{
    public interface IDeveloperProvider : IProvider<Developer>
    {
        public Developer AddDeveloper(string name);
    }

    public class DeveloperProvider(
        IVacationProvider vacationProvider,
        IPlanProvider planProvider,
        IEntwicklungsplanManager entwicklungsplanManager) : IDeveloperProvider
    {
        private ObservableCollection<Developer> Developers { get; } = [];

        public IEnumerable<Developer> GetAll()
        {
            return Developers;
        }

        public void SetAll(IEnumerable<Developer> values)
        {
            Developers.ClearAndAdd(values, new((a, b) => a.Abbreviation.CompareTo(b.Abbreviation)));
        }

        public void Remove(Developer developer)
        {
            vacationProvider.RemoveVacations(developer);
            planProvider.RemovePlans(developer);

            Developers.Remove(developer);
            entwicklungsplanManager.RefreshEntwicklungsplan();
        }

        public Developer AddDeveloper(string name)
        {
            Developer developer = new()
            {
                Name = name,
                Abbreviation = name[0..3].ToUpper(),
            };
            Developers.Add(developer);
            entwicklungsplanManager.RefreshEntwicklungsplan();
            return developer;
        }
    }
}