using DayzServerTools.Application.Models;
using DayzServerTools.Application.Stores;
using DayzServerTools.Application.ViewModels;
using DayzServerTools.Windows.Views;

namespace DayzServerTools.Windows.Models;

internal class WindowsClassnameImportDialog : IClassnameImportDialog
{
    public IClassnameImportStore Store { get; set; }

    public bool? ShowDialog()
    {
        var vm = new ClassnamesImportViewModel(Store);
        var window = new ClassnamesImportView(vm);
        return window.ShowDialog();
    }
}
