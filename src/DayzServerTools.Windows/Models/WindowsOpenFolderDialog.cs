using DayzServerTools.Application.Models;
using Microsoft.Win32;

namespace DayzServerTools.Windows.Models;

internal class WindowsOpenFolderDialog : IFolderDialog
{
    private readonly OpenFolderDialog _dialog;
    public string FileName => _dialog.FolderName;
    public string Title { get =>_dialog.Title; set { _dialog.Title = value; } }

    public bool? ShowDialog()
    {
        _dialog.ShowDialog();
        return true;
    }

    public WindowsOpenFolderDialog()
    {
        _dialog= new OpenFolderDialog();
    }
}
