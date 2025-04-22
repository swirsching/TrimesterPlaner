using Microsoft.Extensions.DependencyInjection;
using Statistics.Models;
using Statistics.ViewModels;
using Statistics.Views;
using System.IO;
using System.Windows;
using TrimesterPlaner.Services;
using Utilities.Extensions;
using Utilities.Utilities;

namespace Statistics
{
    public partial class App : Application
    {
        private ExceptionHandler ExceptionHandler { get; } = new();

        protected override void OnStartup(StartupEventArgs e)
        {
            ConfigService<Config> configService = new(Directory.GetCurrentDirectory());

            ServiceCollection services = new();
            services.AddSingleton(typeof(IConfigService<Config>), configService);

            services.AddTransient(typeof(MainWindow));

            services.AddTransient(typeof(MainWindowViewModel));

            Inject.ServiceProvider = services.BuildServiceProvider();
            Inject.Require<MainWindow>().Show();
        }
    }
}