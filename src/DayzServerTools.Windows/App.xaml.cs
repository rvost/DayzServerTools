using System.Threading.Tasks;
using System.Windows;

using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Squirrel;

using DayzServerTools.Application.Services;
using DayzServerTools.Application.ViewModels;
using DayzServerTools.Windows.Services;

namespace DayzServerTools.Windows
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        protected override void OnStartup(StartupEventArgs e)
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
                    .AddSingleton<WorkspaceViewModel>()
                    .AddTransient<ItemTypesViewModel>()
                    .AddTransient<UserDefinitionsViewModel>()
                    .AddTransient<TraderConfigViewModel>()
                    .AddTransient<ExportViewModel>()
                    .BuildServiceProvider()
                );
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await UpdateApp();

            base.OnExit(e);
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
            await mgr.UpdateApp();
        }
    }
}
