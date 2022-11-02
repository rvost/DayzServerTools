using System.Windows;

using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Squirrel;

using DayzServerTools.Application.Services;
using DayzServerTools.Application.ViewModels;
using DayzServerTools.Application.ViewModels.Panes;
using DayzServerTools.Application.ViewModels.Dialogs;
using DayzServerTools.Application.ViewModels.ItemTypes;
using DayzServerTools.Application.ViewModels.RandomPresets;
using DayzServerTools.Application.ViewModels.SpawnableTypes;
using DayzServerTools.Application.ViewModels.Trader;
using DayzServerTools.Application.ViewModels.UserDefinitions;
using DayzServerTools.Windows.Services;
using DayzServerTools.Library.Common;

namespace DayzServerTools.Windows
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        public App()
        {
            var serviceces = new ServiceCollection();
            ConfigureServices(serviceces);

            var serviceProvider = serviceces.BuildServiceProvider();
            Ioc.Default.ConfigureServices(serviceProvider);
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            SquirrelAwareApp.HandleEvents(
                onInitialInstall: SquirrelConfiguration.OnAppInstall,
                onAppUninstall: SquirrelConfiguration.OnAppUninstall,
                onEveryRun: SquirrelConfiguration.OnAppRun
                );
            await SquirrelConfiguration.UpdateApp();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IDialogFactory, WindowsDialogFactory>();
            services.AddSingleton<IDispatcherService, DispatcherService>();

            services.AddSingleton<WorkspaceViewModel>();
            services.AddSingleton<ErrorsPaneViewModel>();
            services.AddSingleton<ILimitsDefinitionsProvider>(x => 
                x.GetRequiredService<WorkspaceViewModel>());
           
            services.AddTransient<ItemTypesViewModel>();
            services.AddTransient<UserDefinitionsViewModel>();
            services.AddTransient<RandomPresetsViewModel>();
            services.AddTransient<TraderConfigViewModel>();
            services.AddTransient<SpawnableTypesViewModel>();
            services.AddTransient<TraderExportViewModel>();
        }
    }
}
