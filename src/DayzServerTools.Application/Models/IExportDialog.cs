using DayzServerTools.Application.Stores;

namespace DayzServerTools.Application.Models;

public interface IExportDialog
{
    ITraderCategoryExport Store { get; set; }
    bool? ShowDialog();
}
