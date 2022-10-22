using DayzServerTools.Application.Models;

namespace DayzServerTools.Application.Services;

public interface IDialogFactory
{
    IMessageDialog CreateMessageDialog();
    IFileDialog CreateOpenFileDialog();
    IFileDialog CreateSaveFileDialog();
    IExportDialog CreateSpawnableTypesExportDialog();
    ITraderExportDialog CreateTraderExportDialog();
    IClassnameImportDialog CreateClassnameImportDialog();
}
