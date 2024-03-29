﻿using System.Threading.Tasks;
using Squirrel;

namespace DayzServerTools.Windows;

internal static class SquirrelConfiguration
{
    public static void OnAppInstall(SemanticVersion version, IAppTools tools)
    {
        tools.CreateShortcutForThisExe(ShortcutLocation.StartMenu);
    }

    public static void OnAppUninstall(SemanticVersion version, IAppTools tools)
    {
        tools.RemoveShortcutForThisExe(ShortcutLocation.StartMenu);
    }

    public static void OnAppRun(SemanticVersion version, IAppTools tools, bool firstRun)
    {
        tools.SetProcessAppUserModelId();
    }
    public static async Task UpdateApp()
    {
        using var mgr = new GithubUpdateManager("https://github.com/rvost/DayzServerTools");
        if (mgr.IsInstalledApp)
        {
            await mgr.UpdateApp();
        };
    }
}
