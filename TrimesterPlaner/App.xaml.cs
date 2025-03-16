using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Windows;
using TrimesterPlaner.Extensions;
using TrimesterPlaner.Providers;
using TrimesterPlaner.Services;
using TrimesterPlaner.Utilities;
using TrimesterPlaner.ViewModels;
using TrimesterPlaner.Views;

namespace TrimesterPlaner
{
    public partial class App : Application
    {
        private ExceptionHandler ExceptionHandler { get; } = new();

        protected override void OnStartup(StartupEventArgs e)
        {
            ConfigService configService = new(Directory.GetCurrentDirectory());

            ServiceCollection services = new();
            services.AddSingleton(typeof(IConfigService), configService);
            services.AddSingleton(typeof(ConfluenceClient));
            services.AddTransient(typeof(IGenerator), typeof(Generator));
            services.AddSingleton(typeof(JiraClient));
            services.AddTransient(typeof(IPreparator), typeof(Preparator));
            services.AddSingleton(typeof(IPlaner), typeof(Planer));

            services.AddTransient(typeof(IConfigProvider), typeof(ConfigProvider));
            services.AddSingleton(typeof(IDeveloperProvider), typeof(DeveloperProvider));
            services.AddSingleton(typeof(IPlanProvider), typeof(PlanProvider));
            services.AddSingleton(typeof(ISettingsProvider), typeof(SettingsProvider));
            services.AddSingleton(typeof(ITicketProvider), typeof(TicketProvider));
            services.AddSingleton(typeof(IVacationProvider), typeof(VacationProvider));

            services.AddTransient(typeof(ResultWindow));
            services.AddTransient(typeof(ResultWindowViewModel));
            services.AddTransient(typeof(ResultViewModel));
            services.AddTransient(typeof(ResultMenuViewModel));
            services.AddTransient(typeof(MainWindow));
            services.AddTransient(typeof(MainWindowMenuViewModel));
            services.AddTransient(typeof(VacationProviderViewModel));
            services.AddTransient(typeof(TicketProviderViewModel));
            services.AddTransient(typeof(PlanProviderViewModel));
            services.AddTransient(typeof(SettingsViewModel));
            services.AddTransient(typeof(StatisticsViewModel));
            services.AddTransient(typeof(DeveloperProviderViewModel));
            services.AddTransient(typeof(MainWindowViewModel));

            InjectExtension.ServiceProvider = services.BuildServiceProvider();
            InjectExtension.ServiceProvider.GetRequiredService<MainWindow>().Show();

            if (e.Args.Length > 0 && File.Exists(e.Args[0]))
            {
                MainWindowMenuViewModel menuViewModel = new(
                    InjectExtension.ServiceProvider.GetRequiredService<IConfigService>(),
                    InjectExtension.ServiceProvider.GetRequiredService<IConfigProvider>());
                menuViewModel.LoadCommand.Execute(e.Args[0]);
                InjectExtension.ServiceProvider.GetRequiredService<ITicketProvider>().ReloadTicketsAsync();
            }
        }
    }
}