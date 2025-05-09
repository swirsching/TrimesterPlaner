﻿using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Windows;
using TrimesterPlaner.Models;
using TrimesterPlaner.Providers;
using TrimesterPlaner.Services;
using TrimesterPlaner.ViewModels;
using TrimesterPlaner.Views;
using Utilities.Extensions;
using Utilities.Services;
using Utilities.Utilities;

namespace TrimesterPlaner
{
    public partial class App : Application
    {
        private ExceptionHandler ExceptionHandler { get; } = new();

        protected override void OnStartup(StartupEventArgs e)
        {
            ConfigService<Config> configService = new(Directory.GetCurrentDirectory());

            ServiceCollection services = new();
            services.AddSingleton(typeof(IConfigService<Config>), configService);
            services.AddSingleton(typeof(IConfluenceClient), typeof(ConfluenceClient));
            services.AddSingleton(typeof(IJiraClient), typeof(JiraClient));
            services.AddSingleton(typeof(IPlaner), typeof(Planer));
            services.AddTransient(typeof(IGenerator), typeof(Generator));
            services.AddTransient(typeof(IPreparator), typeof(Preparator));

            services.AddSingleton(typeof(IDeveloperProvider), typeof(DeveloperProvider));
            services.AddSingleton(typeof(IPlanProvider), typeof(PlanProvider));
            services.AddSingleton(typeof(ITicketProvider), typeof(TicketProvider));
            services.AddSingleton(typeof(IVacationProvider), typeof(VacationProvider));
            services.AddSingleton(typeof(ISettingsProvider), typeof(SettingsProvider));
            services.AddTransient(typeof(IConfigProvider), typeof(ConfigProvider));

            services.AddTransient(typeof(ResultWindow));
            services.AddTransient(typeof(MainWindow));

            services.AddTransient(typeof(ResultWindowViewModel));
            services.AddTransient(typeof(ResultViewModel));
            services.AddTransient(typeof(ResultMenuViewModel));
            services.AddTransient(typeof(MainWindowMenuViewModel));
            services.AddTransient(typeof(VacationProviderViewModel));
            services.AddTransient(typeof(TicketProviderViewModel));
            services.AddTransient(typeof(PlanProviderViewModel));
            services.AddTransient(typeof(SettingsViewModel));
            services.AddTransient(typeof(StatisticsViewModel));
            services.AddTransient(typeof(DeveloperProviderViewModel));
            services.AddTransient(typeof(MainWindowViewModel));
            services.AddTransient(typeof(TicketDragAdornerViewModel));

            Inject.ServiceProvider = services.BuildServiceProvider();
            Inject.Require<MainWindow>().Show();

            if (e.Args.Length > 0 && File.Exists(e.Args[0]))
            {
                Inject.Require<MainWindowMenuViewModel>().LoadCommand.Execute(e.Args[0]);
                Inject.Require<ITicketProvider>().ReloadTicketsAsync();
            }
        }
    }
}