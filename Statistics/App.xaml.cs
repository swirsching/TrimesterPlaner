using Microsoft.Extensions.DependencyInjection;
using Statistics.Models;
using Statistics.Providers;
using Statistics.ViewModels;
using Statistics.Views;
using System.IO;
using System.Windows;
using Utilities.Extensions;
using Utilities.Services;
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

            services.AddTransient(typeof(IConfigProvider), typeof(ConfigProvider));
            services.AddSingleton(typeof(ISettingsProvider), typeof(SettingsProvider));
            services.AddSingleton(typeof(IStatisticsProvider), typeof(StatisticsProvider));

            services.AddTransient(typeof(MainWindow));

            services.AddTransient(typeof(MainWindowViewModel));
            services.AddTransient(typeof(MenuViewModel));
            services.AddTransient(typeof(SettingsViewModel));

            Inject.ServiceProvider = services.BuildServiceProvider();
            Inject.Require<MainWindow>().Show();

            if (e.Args.Length > 0 && File.Exists(e.Args[0]))
            {
                Inject.Require<MenuViewModel>().LoadCommand.Execute(e.Args[0]);
            }
        }
    }
}