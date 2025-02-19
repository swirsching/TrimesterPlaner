using Microsoft.Win32;
using System.Windows.Input;
using TextCopy;
using TrimesterPlaner.Extensions;
using TrimesterPlaner.Models;
using TrimesterPlaner.Utilities;

namespace TrimesterPlaner.ViewModels
{
    public class ResultMenuViewModel : BindableBase
    {
        public ResultMenuViewModel(ConfluenceClient confluenceClient, IEntwicklungsplanManager entwicklungsplanManager)
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
                    entwicklungsplanManager.GetLastResult()?.Write(dialog.FileName);
                }
            });
            CopyToClipboardCommand = new RelayCommand((o) => ClipboardService.SetText(entwicklungsplanManager.GetLastResult().ConvertToPastableHTML()));
            PushToConfluenceCommand = new RelayCommand((o) => confluenceClient.UpdatePage(entwicklungsplanManager.GetSettings().PageID, entwicklungsplanManager.GetLastResult().ConvertToPastableHTML()));
        }

        public bool HasCAT { get; }
        public ICommand ExportCommand { get; }
        public ICommand CopyToClipboardCommand { get; }
        public ICommand PushToConfluenceCommand { get; }
    }
}