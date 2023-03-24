using DayzServerTools.Application.Models;

namespace DayzServerTools.Application.Services;

public interface IDialogFactory
{
    IConfirmationDialog CreateConfirmationDialog();
    IMessageDialog CreateMessageDialog();
    IFileDialog CreateOpenFileDialog();
    IFileDialog CreateSaveFileDialog();
    IFolderDialog CreateOpenFolderDialog();
    IExportDialog CreateExportDialog();
    IClassnameImportDialog CreateClassnameImportDialog();
}
