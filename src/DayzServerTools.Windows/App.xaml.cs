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

namespace DayzServerTools.Windows
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            SquirrelAwareApp.HandleEvents(
                onInitialInstall: SquirrelConfiguration.OnAppInstall,
                onAppUninstall: SquirrelConfiguration.OnAppUninstall,
                onEveryRun: SquirrelConfiguration.OnAppRun
                );

            Ioc.Default.ConfigureServices
                (new ServiceCollection()
                    .AddSingleton<IDialogFactory, WindowsDialogFactory>()
                    .AddSingleton<IDispatcherService, DispatcherService>()
                    .AddSingleton<WorkspaceViewModel>()
                    .AddSingleton<ErrorsPaneViewModel>()
                    .AddTransient<ItemTypesViewModel>()
                    .AddTransient<UserDefinitionsViewModel>()
                    .AddTransient<RandomPresetsViewModel>()
                    .AddTransient<TraderConfigViewModel>()
                    .AddTransient<SpawnableTypesViewModel>()
                    .AddTransient<TraderExportViewModel>()
                    .BuildServiceProvider()
                );

            await SquirrelConfiguration.UpdateApp();
        }
    }
}
