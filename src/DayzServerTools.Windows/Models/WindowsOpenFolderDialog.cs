using DayzServerTools.Application.Models;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace DayzServerTools.Windows.Models;

internal class WindowsOpenFolderDialog : IFolderDialog
{
    private readonly CommonOpenFileDialog _dialog;
    public string FileName => _dialog.FileName;
    public string Title { get =>_dialog.Title; set { _dialog.Title = value; } }

    public bool? ShowDialog()
    {
        _dialog.ShowDialog();
        return true;
    }

    public WindowsOpenFolderDialog()
    {
        _dialog= new CommonOpenFileDialog() { IsFolderPicker=true};
    }
}
