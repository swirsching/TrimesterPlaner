using System.Windows.Input;
using TrimesterPlaner.Utilities;

namespace TrimesterPlaner.ViewModels
{
    public class MainWindowMenuViewModel(IConfigService configService, IConfigManager configManager) : BindableBase
    {
        public ICommand LoadCommand { get; } = new RelayCommand((o) => configManager.Load(configService.LoadConfig(o as string)));
        public ICommand SaveCommand { get; } = new RelayCommand((o) => configService.SaveConfig(configManager.Save()));
        public ICommand SaveCopyCommand { get; } = new RelayCommand((o) => configService.SaveConfigCopy(configManager.Save()));
    }
}