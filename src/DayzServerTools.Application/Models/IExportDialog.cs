using DayzServerTools.Library.Xml;

namespace DayzServerTools.Application.Models;

public interface IExportDialog
{
    IEnumerable<ItemType> Items { get; set; }
    bool? ShowDialog();
}
