using System.Windows.Input;
using TrimesterPlaner.Models;
using TrimesterPlaner.Providers;
using TrimesterPlaner.Services;
using TrimesterPlaner.Utilities;
using Utilities.Utilities;

namespace TrimesterPlaner.ViewModels
{
    public class MainWindowMenuViewModel(IConfigService<Config> configService, IConfigProvider configProvider) : BindableBase
    {
        public ICommand LoadCommand { get; } = new RelayCommand((o) => configProvider.Set(configService.LoadConfig(o as string)));
        public ICommand SaveCommand { get; } = new RelayCommand((o) => configService.SaveConfig(configProvider.Get()));
        public ICommand SaveCopyCommand { get; } = new RelayCommand((o) => configService.SaveConfigCopy(configProvider.Get()));
    }
}