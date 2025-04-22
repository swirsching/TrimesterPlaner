using Microsoft.Win32;
using System.Windows.Input;
using TextCopy;
using TrimesterPlaner.Extensions;
using TrimesterPlaner.Providers;
using TrimesterPlaner.Services;
using TrimesterPlaner.Utilities;
using Utilities.Extensions;
using Utilities.Services;
using Utilities.Utilities;

namespace TrimesterPlaner.ViewModels
{
    public class ResultMenuViewModel : BindableBase
    {
        public ResultMenuViewModel()
        {
            HasCAT = Inject.Require<IConfluenceClient>().HasCAT();

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
                    Inject.Require<IPlaner>().GetLastPlan()?.Write(dialog.FileName);
                }
            });
            CopyToClipboardCommand = new RelayCommand((o) =>
            {
                ClipboardService.SetText(Inject.Require<IPlaner>().GetLastPlan().ConvertToPastableHTML());
            });
            PushToConfluenceCommand = new RelayCommand((o) =>
            {
                Inject.Require<IConfluenceClient>().UpdatePage(
                    Inject.Require<ISettingsProvider>().Get().PageID,
                    Inject.Require<IPlaner>().GetLastPlan().ConvertToPastableHTML());
            });
        }

        public bool HasCAT { get; }
        public ICommand ExportCommand { get; }
        public ICommand CopyToClipboardCommand { get; }
        public ICommand PushToConfluenceCommand { get; }
    }
}