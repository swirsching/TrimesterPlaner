﻿using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Windows;
using TrimesterPlaner.Converters;
using TrimesterPlaner.Extensions;
using TrimesterPlaner.Models;
using TrimesterPlaner.Utilities;
using TrimesterPlaner.ViewModels;
using TrimesterPlaner.Views;

namespace TrimesterPlaner
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            ConfigService configService = new(Directory.GetCurrentDirectory());

            ServiceCollection services = new();
            services.AddSingleton(typeof(IConfigService), configService);
            services.AddSingleton(typeof(JiraClient));
            services.AddSingleton(typeof(DeveloperProviderViewModel));

            services.AddTransient(typeof(IGenerator), typeof(Generator));
            services.AddTransient(typeof(IPreparator), typeof(Preparator));
            services.AddTransient(typeof(MainWindow));
            services.AddTransient(typeof(VacationProviderViewModel));
            services.AddTransient(typeof(TicketProviderViewModel));
            services.AddTransient(typeof(PlanProviderViewModel));
            services.AddTransient(typeof(SettingsViewModel));
            services.AddTransient(typeof(StatisticsViewModel));
            
            var tmpServiceProvider = services.BuildServiceProvider();
            MainWindowViewModel mainWindowViewModel = new(
                tmpServiceProvider.GetRequiredService<JiraClient>(),
                tmpServiceProvider.GetRequiredService<IGenerator>(),
                tmpServiceProvider.GetRequiredService<IPreparator>(),
                tmpServiceProvider.GetRequiredService<IConfigService>());
            if (e.Args.Length > 0)
            {
                mainWindowViewModel.LoadCommand.Execute(e.Args[0]);
                _ = mainWindowViewModel.ReloadTicketsAsync();
            }
            services.AddSingleton(mainWindowViewModel);
            services.AddSingleton(typeof(IEntwicklungsplanManager), mainWindowViewModel);
            services.AddSingleton(typeof(IDeveloperManager), mainWindowViewModel);
            services.AddSingleton(typeof(IVacationManager), mainWindowViewModel);
            services.AddSingleton(typeof(ITicketManager), mainWindowViewModel);
            services.AddSingleton(typeof(IPlanManager), mainWindowViewModel);
            services.AddSingleton(typeof(ISettingsProvider), mainWindowViewModel);
            services.AddSingleton(typeof(IDeveloperProvider), mainWindowViewModel);
            services.AddSingleton(typeof(IVacationProvider), mainWindowViewModel);
            services.AddSingleton(typeof(ITicketProvider), mainWindowViewModel);
            services.AddSingleton(typeof(IPlanProvider), mainWindowViewModel);

            var serviceProvider = services.BuildServiceProvider();
            InjectExtension.ServiceProvider = serviceProvider;
            Injector.ServiceProvider = serviceProvider;

            serviceProvider.GetRequiredService<MainWindow>().Show();
        }
    }
}