using DayzServerTools.Application.Models;
using DayzServerTools.Application.ViewModels.Dialogs;
using DayzServerTools.Windows.Views.Dialogs;

namespace DayzServerTools.Windows.Models;

internal class WindowsExportDialog : IExportDialog
{
    public bool? ShowDialog(IExportViewModel vm)
    {
        var window = new ExportView(vm);

        return window.ShowDialog();
    }
}
