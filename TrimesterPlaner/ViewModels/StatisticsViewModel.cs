﻿using System.Collections.ObjectModel;
using TrimesterPlaner.Extensions;
using TrimesterPlaner.Models;
using TrimesterPlaner.Utilities;

namespace TrimesterPlaner.ViewModels
{
    public record Statistics(
        string Label,
        double CapacityPT,
        double VacationPT,
        double TicketsPT,
        double FehlerPT,
        double SpecialPT,
        double RemainingPT);

    public class StatisticsViewModel : BindableBase
    {
        public StatisticsViewModel(IDeveloperProvider developerProvider, ISettingsProvider settingsProvider, IEntwicklungsplanManager entwicklungsplanManager)
        {
            DeveloperProvider = developerProvider;
            SettingsProvider = settingsProvider;

            entwicklungsplanManager.EntwicklungsplanChanged += CalculateStatistics;
            CalculateStatistics();
        }

        private void CalculateStatistics(PreparedData? data = null)
        {
            CalculateDeveloperStatistics();
            if (data is not null)
            {
                CalculateWeekStatistics(data);
            }
        }

        private void CalculateDeveloperStatistics()
        {
            DeveloperStatistics.Clear();

            var developers = DeveloperProvider.GetDevelopers();
            var settings = SettingsProvider.GetSettings();
            double totalCapacityPT = (from developer in developers
                                      from day in settings.GetDaysOfEntwicklungsperiode()
                                      where developer.IsWorkDay(day)
                                      select developer.GetDailyPT()).Sum();
            double totalVacationPT = 0, totalTicketsPT = 0, totalFehlerPT = 0, totalSpecialPT = 0, totalRemainingPT = 0;
            foreach (var developer in developers)
            {
                double dailyPT = developer.GetDailyPT();

                var capacityPT = from day in settings.GetDaysOfEntwicklungsperiode()
                                 where developer.IsWorkDay(day)
                                 select dailyPT;
                var sumCapacityPT = capacityPT.Sum();
                var capacityPercentage = sumCapacityPT / totalCapacityPT;

                var vacationPT = from vacation in developer.Vacations
                                 from day in vacation.GetDays()
                                 where developer.IsRegularWorkDay(day)
                                 select dailyPT;
                var sumVacationPT = vacationPT.Sum();
                totalVacationPT += sumVacationPT;

                var ticketsPT = from plan in developer.Plans 
                               where plan is TicketPlan 
                               select plan.GetTotalPT();
                var sumTicketsPT = ticketsPT.Sum();
                totalTicketsPT += sumTicketsPT;

                var fehlerPT = from plan in developer.Plans 
                               where plan is BugPlan 
                               select plan.GetTotalPT();
                var sumFehlerPT = fehlerPT.Sum();
                var fehlerSoll = capacityPercentage * settings.FehlerPT;
                totalFehlerPT += sumFehlerPT;

                var specialPT = from plan in developer.Plans 
                                where plan is SpecialPlan 
                                select plan.GetTotalPT();
                var sumSpecialPT = specialPT.Sum();
                totalSpecialPT += sumSpecialPT;

                var remainingPT = sumCapacityPT - sumTicketsPT - sumFehlerPT - sumSpecialPT;
                totalRemainingPT += remainingPT;

                DeveloperStatistics.Add(new Statistics(
                    developer.Abbreviation,
                    Math.Round(sumCapacityPT, 1),
                    Math.Round(sumVacationPT, 1),
                    Math.Round(sumTicketsPT, 1),
                    Math.Round(sumFehlerPT - fehlerSoll, 1),
                    Math.Round(sumSpecialPT, 1),
                    Math.Round(remainingPT, 1)));
            }

            DeveloperStatistics.Add(new Statistics(
                "∑",
                Math.Round(totalCapacityPT, 1),
                Math.Round(totalVacationPT, 1),
                Math.Round(totalTicketsPT, 1),
                Math.Round(totalFehlerPT - settings.FehlerPT, 1),
                Math.Round(totalSpecialPT, 1),
                Math.Round(totalRemainingPT, 1)));
        }

        private void CalculateWeekStatistics(PreparedData data)
        {
            WeekStatistics.Clear();

            double totalCapacityPT = 0, totalVacationPT = 0, totalTicketsPT = 0, totalFehlerPT = 0, totalSpecialPT = 0, totalRemainingPT = 0;
            foreach (var week in data.Weeks)
            {
                var capacityPT = week.Days.First().Capacity - week.Days.Last().Capacity;
                totalCapacityPT += capacityPT;

                var vacationPT = from day in week.Days
                                 from vacation in day.Vacations
                                 select vacation.Value;
                var sumVacationPT = vacationPT.Sum();
                totalVacationPT += sumVacationPT;

                var ticketsPT = from day in week.Days
                                from plan in day.Plans
                                where plan.Key.PlanType == PlanType.Ticket
                                select plan.Value;
                var sumTicketsPT = ticketsPT.Sum();
                totalTicketsPT += sumTicketsPT;

                var fehlerPT = from day in week.Days
                               from plan in day.Plans
                               where plan.Key.PlanType == PlanType.Bug
                               select plan.Value;
                var sumFehlerPT = fehlerPT.Sum();
                totalFehlerPT += sumFehlerPT;

                var specialPT = from day in week.Days
                                from plan in day.Plans
                                where plan.Key.PlanType == PlanType.Special
                                select plan.Value;
                var sumSpecialPT = specialPT.Sum(); 
                totalSpecialPT += sumSpecialPT;

                WeekStatistics.Add(new Statistics(
                    $"KW {week.Weeknum}",
                    Math.Round(capacityPT, 1),
                    Math.Round(sumVacationPT, 1),
                    Math.Round(sumTicketsPT, 1),
                    Math.Round(sumFehlerPT, 1),
                    Math.Round(sumSpecialPT, 1),
                    Math.Round(0.0, 1)));
            }

            WeekStatistics.Add(new Statistics(
                "∑",
                Math.Round(totalCapacityPT, 1),
                Math.Round(totalVacationPT, 1),
                Math.Round(totalTicketsPT, 1),
                Math.Round(totalFehlerPT, 1),
                Math.Round(totalSpecialPT, 1),
                Math.Round(totalRemainingPT, 1)));
        }

        private IDeveloperProvider DeveloperProvider { get; }
        private ISettingsProvider SettingsProvider { get; }

        public ObservableCollection<Statistics> DeveloperStatistics { get; } = [];
        public ObservableCollection<Statistics> WeekStatistics { get; } = [];
    }
}