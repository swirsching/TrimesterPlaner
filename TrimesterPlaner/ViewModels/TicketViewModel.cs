using System.Diagnostics;
using System.Windows.Input;
using TrimesterPlaner.Extensions;
using TrimesterPlaner.Models;
using TrimesterPlaner.Utilities;

namespace TrimesterPlaner.ViewModels
{
    public class TicketViewModel : BindableBase
    {
        public TicketViewModel(
            Ticket ticket, 
            ITicketManager ticketManager, 
            IPlanManager planManager, 
            DeveloperProviderViewModel developerProviderViewModel,
            IEntwicklungsplanManager entwicklungsplanManager) : base()
        { 
            Ticket = ticket;

            OpenTicketInBrowserCommand = new RelayCommand((o) => OpenTicketInBrowser());
            RemoveTicketCommand = new RelayCommand((o) => ticketManager.RemoveTicket(ticket));
            AddPlanCommand = new RelayCommand((o) => planManager.AddTicketPlan(SelectedDeveloper!, Ticket));

            developerProviderViewModel.OnSelectedDeveloperChanged += OnSelectedDeveloperChanged;
            entwicklungsplanManager.EntwicklungsplanChanged += CalculatePlannedPercentage;
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

        private void CalculatePlannedPercentage(PreparedData? data = null)
        {
            PlannedPercentage = Ticket.GetPlannedPT() / TicketPT;
        }

        private double _PlannedPercentage;
        public double PlannedPercentage
        {
            get => _PlannedPercentage;
            set => SetProperty(ref _PlannedPercentage, value);
        }

        public Ticket Ticket { get; }
        public double TicketPT { get => Ticket.GetTotalPT(); }

        public ICommand OpenTicketInBrowserCommand { get; }
        public ICommand RemoveTicketCommand { get; }
        public ICommand AddPlanCommand { get; }

        private bool _ShowDetails;
        public bool ShowDetails
        {
            get => _ShowDetails;
            set => SetProperty(ref _ShowDetails, value);
        }

        private void OpenTicketInBrowser()
        {
            Process.Start(new ProcessStartInfo($"https://confluence.ivu.de/jira/browse/{Ticket.Key}") { UseShellExecute = true });
        }
    }
}