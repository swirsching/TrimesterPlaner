﻿using System.Windows.Input;
using TrimesterPlaner.Extensions;
using TrimesterPlaner.Models;
using TrimesterPlaner.Utilities;

namespace TrimesterPlaner.ViewModels
{
    public class TicketViewModel : BindableBase
    {
        public TicketViewModel(Ticket ticket, ITicketManager ticketManager, IPlanManager planManager, DeveloperProviderViewModel developerProviderViewModel) : base()
        { 
            Ticket = ticket;

            RemoveTicketCommand = new RelayCommand((o) => ticketManager.RemoveTicket(ticket));
            AddPlanCommand = new RelayCommand((o) => planManager.AddTicketPlan(SelectedDeveloper!, Ticket));

            developerProviderViewModel.OnSelectedDeveloperChanged += OnSelectedDeveloperChanged;
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

        public Ticket Ticket { get; }
        public double TicketPT { get => Ticket.GetTotalPT(); }

        public ICommand RemoveTicketCommand { get; }
        public ICommand AddPlanCommand { get; }

        private bool _ShowDetails;
        public bool ShowDetails
        {
            get => _ShowDetails;
            set => SetProperty(ref _ShowDetails, value);
        }
    }
}