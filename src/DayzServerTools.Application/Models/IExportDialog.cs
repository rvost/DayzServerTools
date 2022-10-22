using DayzServerTools.Application.ViewModels.Dialogs;

namespace DayzServerTools.Application.Models;

public interface IExportDialog
{
    bool? ShowDialog(IExportViewModel vm);
}
