using System;
using System.Collections.Generic;

using CommunityToolkit.Mvvm.DependencyInjection;

using DayzServerTools.Application.Models;
using DayzServerTools.Application.ViewModels;
using DayzServerTools.Library.Xml;
using DayzServerTools.Windows.Views;


namespace DayzServerTools.Windows.Models;

internal class WindowsExportDialog : IExportDialog
{
    public IEnumerable<ItemType> Items { get; set; }

    public bool? ShowDialog()
    {
        var vm =Ioc.Default.GetService<ExportViewModel>();
        vm.Items = Items;

        var window = new ExportDialogWindow(vm);
        
        return window.ShowDialog();
    }
}
