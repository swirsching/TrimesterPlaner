using System.Diagnostics;
using System.Windows.Input;
using TrimesterPlaner.Extensions;
using TrimesterPlaner.Models;
using TrimesterPlaner.Providers;
using TrimesterPlaner.Services;
using TrimesterPlaner.Utilities;

namespace TrimesterPlaner.ViewModels
{
    public class TicketViewModel : BindableBase
    {
        public TicketViewModel(Ticket ticket)
        {
            Ticket = ticket;

            OpenTicketInBrowserCommand = new RelayCommand((o) => OpenTicketInBrowser());
            AddPlanCommand = new RelayCommand((o) => Inject.Require<IPlanProvider>().AddTicketPlan(SelectedDeveloper!, Ticket));
            RemoveCommand = new RelayCommand((o) => Inject.Require<ITicketProvider>().Remove(Ticket));

            Inject.Require<IDeveloperProvider>().OnSelectedDeveloperChanged += OnSelectedDeveloperChanged;
            Inject.Require<IPlaner>().PlanChanged += (data, result) => CalculatePlannedPercentage();
            CalculatePlannedPercentage();
        }

        private void OnSelectedDeveloperChanged(Developer? selectedDeveloper)
        {
            SelectedDeveloper = selectedDeveloper;
        }

        private Developer? _SelectedDeveloper;
        public Developer? SelectedDeveloper
        {
            get => _SelectedDeveloper;
            set => SetProperty(ref _SelectedDeveloper, value);
        }

        private void CalculatePlannedPercentage()
        {
            PlannedPercentage = Ticket.GetPlannedPT() / Ticket.GetTotalPT();
        }

        private double _PlannedPercentage;
        public double PlannedPercentage
        {
            get => _PlannedPercentage;
            set => SetProperty(ref _PlannedPercentage, value);
        }

        public Ticket Ticket { get; }
        public string Key { get => Ticket.Key; }
        public string Summary { get => Ticket.Summary; }
        public bool IsInJQL { get => Ticket.IsInJQL; }

        public ICommand OpenTicketInBrowserCommand { get; }
        public ICommand AddPlanCommand { get; }
        public ICommand RemoveCommand { get; }

        private void OpenTicketInBrowser()
        {
            Process.Start(new ProcessStartInfo($"https://confluence.ivu.de/jira/browse/{Ticket.Key}") { UseShellExecute = true });
        }
    }
}