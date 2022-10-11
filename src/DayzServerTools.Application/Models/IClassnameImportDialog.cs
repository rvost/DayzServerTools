using DayzServerTools.Application.Stores;

namespace DayzServerTools.Application.Models;

public interface IClassnameImportDialog
{
    IClassnameImportStore Store { get; set; }
    bool? ShowDialog();
}
