using CommunityToolkit.Mvvm.DependencyInjection;

using DayzServerTools.Application.Models;
using DayzServerTools.Application.Stores;
using DayzServerTools.Application.ViewModels;
using DayzServerTools.Windows.Views;


namespace DayzServerTools.Windows.Models;

internal class WindowsExportDialog : IExportDialog
{
    public ITraderCategoryExport Store { get; set; }

    public bool? ShowDialog()
    {
        var vm =Ioc.Default.GetService<ExportViewModel>();
        vm.Store = Store;

        var window = new ExportDialogWindow(vm);
        
        return window.ShowDialog();
    }
}
