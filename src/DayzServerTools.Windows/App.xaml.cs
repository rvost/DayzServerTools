using System.Threading.Tasks;
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
                onInitialInstall: OnAppInstall,
                onAppUninstall: OnAppUninstall,
                onEveryRun: OnAppRun
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

            await UpdateApp();
        }

        private static void OnAppInstall(SemanticVersion version, IAppTools tools)
        {
            tools.CreateShortcutForThisExe(ShortcutLocation.StartMenu);
        }

        private static void OnAppUninstall(SemanticVersion version, IAppTools tools)
        {
            tools.RemoveShortcutForThisExe(ShortcutLocation.StartMenu);
        }

        private static void OnAppRun(SemanticVersion version, IAppTools tools, bool firstRun)
        {
            tools.SetProcessAppUserModelId();
        }
        private static async Task UpdateApp()
        {
            using var mgr = new GithubUpdateManager("https://github.com/rvost/DayzServerTools");
            if (mgr.IsInstalledApp)
            {
                 await mgr.UpdateApp();
            };
        }
    }
}
