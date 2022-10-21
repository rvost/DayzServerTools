using CommunityToolkit.Mvvm.DependencyInjection;

using DayzServerTools.Application.Models;
using DayzServerTools.Application.Stores;
using DayzServerTools.Application.ViewModels.Dialogs;
using DayzServerTools.Windows.Views.Dialogs;


namespace DayzServerTools.Windows.Models;

internal class WindowsTraderExportDialog : ITraderExportDialog
{
    public ITraderCategoryExport Store { get; set; }

    public bool? ShowDialog()
    {
        var vm =Ioc.Default.GetService<TraderExportViewModel>();
        vm.Store = Store;

        var window = new TraderExportDialogWindow(vm);
        
        return window.ShowDialog();
    }
}
