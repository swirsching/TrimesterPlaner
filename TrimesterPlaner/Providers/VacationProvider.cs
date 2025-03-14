using System.Collections.ObjectModel;
using TrimesterPlaner.Extensions;
using TrimesterPlaner.Models;
using TrimesterPlaner.ViewModels;

namespace TrimesterPlaner.Providers
{
    public interface IVacationProvider : IProvider<Vacation>
    {
        public void AddVacation(Developer developer);
        public void RemoveVacations(Developer developer);
    }

    public class VacationProvider(IEntwicklungsplanManager entwicklungsplanManager) : IVacationProvider
    {
        private ObservableCollection<Vacation> Vacations { get; } = [];

        public IEnumerable<Vacation> GetAll()
        {
            return Vacations;
        }

        public void SetAll(IEnumerable<Vacation> vacations)
        {
            Vacations.ClearAndAdd(vacations, new((a, b) =>
            {
                if (a.Start is null || b.Start is null || a.Start == b.Start)
                {
                    if (a.Developer is null || b.Developer is null)
                    {
                        return 0;
                    }

                    return a.Developer.Abbreviation.CompareTo(b.Developer.Abbreviation);
                }

                return a.Start < b.Start ? -1 : 1;
            }));
        }

        public void Remove(Vacation vacation)
        {
            vacation.Developer = null;
            Vacations.Remove(vacation);
            entwicklungsplanManager.RefreshEntwicklungsplan();
        }

        public void AddVacation(Developer developer)
        {
            Vacations.Add(new Vacation() { Developer = developer });
            entwicklungsplanManager.RefreshEntwicklungsplan();
        }

        public void RemoveVacations(Developer developer)
        {
            List<Vacation> vacationsToRemove = [.. developer.Vacations];
            foreach (var vacation in vacationsToRemove)
            {
                vacation.Developer = null;
                Vacations.Remove(vacation);
            }
            entwicklungsplanManager.RefreshEntwicklungsplan();
        }
    }
}