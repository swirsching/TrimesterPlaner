using GongSolutions.Wpf.DragDrop;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using TrimesterPlaner.Extensions;
using TrimesterPlaner.Models;
using TrimesterPlaner.Providers;
using TrimesterPlaner.Utilities;

namespace TrimesterPlaner.ViewModels
{
    public class PlanProviderViewModel : BindableBase, IDropTarget
    {
        public PlanProviderViewModel()
        {
            Plans = new() { Source = Inject.Require<IPlanProvider>().Get() };
            Plans.Filter += FilterBySelectedDeveloper;
            Inject.Require<IDeveloperProvider>().OnSelectedDeveloperChanged += OnSelectedDeveloperChanged;

            AddBugPlanCommand = new RelayCommand((o) => Inject.Require<IPlanProvider>().AddBugPlan(SelectedDeveloper!));
            AddSpecialPlanCommand = new RelayCommand((o) => Inject.Require<IPlanProvider>().AddSpecialPlan(SelectedDeveloper!));
        }

        private void FilterBySelectedDeveloper(object sender, FilterEventArgs e)
        {
            e.Accepted = SelectedDeveloper is null || SelectedDeveloper == ((Plan)e.Item).Developer;
        }

        private void OnSelectedDeveloperChanged(Developer? selectedDeveloper)
        {
            SelectedDeveloper = selectedDeveloper;
            PlansView.Refresh();
        }

        private Developer? _SelectedDeveloper;
        public Developer? SelectedDeveloper
        {
            get => _SelectedDeveloper;
            set => SetProperty(ref _SelectedDeveloper, value);
        }

        private CollectionViewSource Plans { get; }

        public ICollectionView PlansView
        {
            get => Plans.View;
        }

        public void DragOver(IDropInfo dropInfo)
        {
            if (dropInfo.Data is not Plan || dropInfo.TargetItem is not Plan)
            {
                return;
            }

            dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
            dropInfo.Effects = DragDropEffects.Move;
        }

        public void Drop(IDropInfo dropInfo)
        {
            Inject.Require<IPlanProvider>().Move((Plan)dropInfo.Data, (Plan)dropInfo.TargetItem);
        }

        public ICommand AddBugPlanCommand { get; }
        public ICommand AddSpecialPlanCommand { get; }
    }
}