using DayzServerTools.Application.Stores;

namespace DayzServerTools.Application.Models;

public interface ITraderExportDialog
{
    ITraderCategoryExport Store { get; set; }
    bool? ShowDialog();
}
