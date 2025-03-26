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
            RemoveCommand = new RelayCommand((o) => Inject.Require<ITicketProvider>().Remove(Ticket));

            Inject.Require<IPlaner>().PlanChanged += (data, result) => CalculatePlannedPercentage();
            CalculatePlannedPercentage();
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
        public ICommand RemoveCommand { get; }

        private void OpenTicketInBrowser()
        {
            Process.Start(new ProcessStartInfo($"https://confluence.ivu.de/jira/browse/{Ticket.Key}") { UseShellExecute = true });
        }
    }
}