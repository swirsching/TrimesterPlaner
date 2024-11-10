using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;

namespace TrimesterPlaner.Utilities
{
    public class ExceptionHandler
    {
        public ExceptionHandler() 
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, args) => HandleException((Exception)args.ExceptionObject);
            TaskScheduler.UnobservedTaskException += (sender, args) => HandleException(args.Exception);
            Dispatcher.CurrentDispatcher.UnhandledException += (sender, args) => HandleException(args.Exception);
        }

        private static void HandleException(Exception e)
        {
            if (!Debugger.IsAttached)
            {
                MessageBox.Show(e.Message, "Error");
                Application.Current.Shutdown();
            }
        }
    }
}