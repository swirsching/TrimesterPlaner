using Microsoft.Win32;
using System.Windows.Input;
using TextCopy;
using TrimesterPlaner.Extensions;
using TrimesterPlaner.Providers;
using TrimesterPlaner.Services;
using TrimesterPlaner.Utilities;

namespace TrimesterPlaner.ViewModels
{
    public class ResultMenuViewModel : BindableBase
    {
        public ResultMenuViewModel(
            ConfluenceClient confluenceClient, 
            ISettingsProvider settingsProvider,
            IPlaner trimesterPlaner)
        {
            HasCAT = confluenceClient.HasCAT;

            ExportCommand = new RelayCommand((o) =>
            {
                var dialog = new SaveFileDialog()
                {
                    FileName = "TrimesterPlaner",
                    DefaultExt = ".svg",
                };

                bool? ok = dialog.ShowDialog();
                if (ok == true)
                {
                    trimesterPlaner.GetLastPlan()?.Write(dialog.FileName);
                }
            });
            CopyToClipboardCommand = new RelayCommand((o) => ClipboardService.SetText(trimesterPlaner.GetLastPlan().ConvertToPastableHTML()));
            PushToConfluenceCommand = new RelayCommand((o) => confluenceClient.UpdatePage(settingsProvider.Get().PageID, trimesterPlaner.GetLastPlan().ConvertToPastableHTML()));
        }

        public bool HasCAT { get; }
        public ICommand ExportCommand { get; }
        public ICommand CopyToClipboardCommand { get; }
        public ICommand PushToConfluenceCommand { get; }
    }
}