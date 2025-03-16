﻿using System.Collections.ObjectModel;
using TrimesterPlaner.Extensions;
using TrimesterPlaner.Models;
using TrimesterPlaner.Services;
using TrimesterPlaner.ViewModels;

namespace TrimesterPlaner.Providers
{
    public interface IVacationProvider : ICollectionProvider<Vacation>
    {
        public void AddVacation(Developer developer);
        public void RemoveVacations(Developer developer);
    }

    public class VacationProvider(IPlaner trimesterPlaner) : IVacationProvider
    {
        private ObservableCollection<Vacation> Vacations { get; } = [];

        public IEnumerable<Vacation> Get()
        {
            return Vacations;
        }

        public void Set(IEnumerable<Vacation> vacations)
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

        public void Remove(Vacation value)
        {
            RemoveVacation(value);
        }

        public void AddVacation(Developer developer)
        {
            Vacations.Add(new Vacation() { Developer = developer });
            trimesterPlaner.RefreshPlan();
        }

        public void RemoveVacations(Developer developer)
        {
            List<Vacation> vacationsToRemove = [.. developer.Vacations];
            foreach (var vacation in vacationsToRemove)
            {
                RemoveVacation(vacation, false);
            }
            trimesterPlaner.RefreshPlan();
        }

        private void RemoveVacation(Vacation vacation, bool refresh = true)
        {
            vacation.Developer = null;
            Vacations.Remove(vacation);
            if (refresh)
            {
                trimesterPlaner.RefreshPlan();
            }
        }
    }
}