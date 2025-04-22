using Statistics.Models;
using Statistics.Providers;
using System.Windows.Input;
using Utilities.Services;
using Utilities.Utilities;

namespace Statistics.ViewModels
{
    public class MenuViewModel(IConfigService<Config> configService, IConfigProvider configProvider) : PropertyChangedBase
    {
        private static string SuggestedFileName { get; } = "Statistics";

        public ICommand LoadCommand { get; } = new RelayCommand((o) => configProvider.Set(configService.LoadConfig(o as string)));
        public ICommand SaveCommand { get; } = new RelayCommand((o) => configService.SaveConfig(configProvider.Get(), SuggestedFileName));
        public ICommand SaveCopyCommand { get; } = new RelayCommand((o) => configService.SaveConfigCopy(configProvider.Get(), SuggestedFileName));
    }
}