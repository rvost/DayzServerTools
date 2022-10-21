using DayzServerTools.Application.Services;
using DayzServerTools.Application.Models;
using DayzServerTools.Windows.Models;

namespace DayzServerTools.Windows.Services;

internal class WindowsDialogFactory : IDialogFactory
{
    private string defaultFilter = "XML Files (*.xml)|*.xml";

    public IClassnameImportDialog CreateClassnameImportDialog()
        => new WindowsClassnameImportDialog();

    public ITraderExportDialog CreateTraderExportDialog() 
        => new WindowsTraderExportDialog();

    public IMessageDialog CreateMessageDialog() 
        => new WindowsMessageDialog();

    public IFileDialog CreateOpenFileDialog() 
        => new WindowsOpenFileDialog() { Filter = defaultFilter };

    public IFileDialog CreateSaveFileDialog() 
        => new WindowsSaveFileDialog() { Filter = defaultFilter };
}
