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
            IPlanManager planManager, 
            DeveloperProviderViewModel developerProviderViewModel,
            IEntwicklungsplanManager entwicklungsplanManager) : base()
        { 
            Ticket = ticket;

            OpenTicketInBrowserCommand = new RelayCommand((o) => OpenTicketInBrowser());
            ShowPlansCommand = new RelayCommand((o) => ShowPlans());
            ShowDetailsCommand = new RelayCommand((o) => ShowDetails());
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
            PlannedPercentage = Ticket.GetPlannedPT() / Ticket.GetTotalPT();
        }

        private double _PlannedPercentage;
        public double PlannedPercentage
        {
            get => _PlannedPercentage;
            set => SetProperty(ref _PlannedPercentage, value);
        }

        public Ticket Ticket { get; }

        public ICommand OpenTicketInBrowserCommand { get; }
        public ICommand ShowPlansCommand { get; }
        public ICommand ShowDetailsCommand { get; }
        public ICommand AddPlanCommand { get; }

        private void ShowPlans()
        {
            throw new NotImplementedException();
        }

        private void ShowDetails()
        {
            throw new NotImplementedException();
        }

        private void OpenTicketInBrowser()
        {
            Process.Start(new ProcessStartInfo($"https://confluence.ivu.de/jira/browse/{Ticket.Key}") { UseShellExecute = true });
        }
    }
}