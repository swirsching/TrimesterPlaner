﻿using TrimesterPlaner.Extensions;
using TrimesterPlaner.Models;

namespace TrimesterPlaner.ViewModels
{
    public class DeveloperViewModel : BaseViewModel
    {
        public DeveloperViewModel(Developer developer, IEntwicklungsplanManager entwicklungsplanManager) : base(entwicklungsplanManager)
        {
            Developer = developer;
            WorkDays = from dayOfWeek in Enumerable.Range((int)DayOfWeek.Monday, 5)
                       select new WorkDayViewModel((DayOfWeek)(dayOfWeek % 7), Developer, entwicklungsplanManager);

            entwicklungsplanManager.EntwicklungsplanChanged += (data, result) => CalculatePT();
            CalculatePT();
        }

        private Developer Developer { get; }

        public string Name
        {
            get => Developer.Name;
            set 
            {
                Developer.Name = value;
                OnPropertyChanged();
            }
        }

        public string Abbreviation
        {
            get => Developer.Abbreviation;
            set
            {
                Developer.Abbreviation = value;
                OnPropertyChanged();
            }
        }

        public int FTE
        {
            get => Developer.FTE;
            set
            {
                Developer.FTE = value;
                OnPropertyChanged();
            }
        }

        public int Sonderrolle
        {
            get => Developer.Sonderrolle;
            set
            {
                Developer.Sonderrolle = value;
                OnPropertyChanged();
            }
        }

        public int Verwaltung
        {
            get => Developer.Verwaltung;
            set
            {
                Developer.Verwaltung = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<WorkDayViewModel> WorkDays { get; }

        private void CalculatePT()
        {
            var dailyPT = Developer.GetDailyPT();
            DailyPT = Math.Round(dailyPT, 1);
            WeeklyPT = Math.Round(dailyPT * (7 - Developer.FreeDays.Count), 1);
        }

        private double _DailyPT = 0.0;
        public double DailyPT
        {
            get => _DailyPT;
            set => SetProperty(ref _DailyPT, value);
        }

        private double _WeeklyPT = 0.0;
        public double WeeklyPT
        {
            get => _WeeklyPT;
            set => SetProperty(ref _WeeklyPT, value);
        }
    }
}