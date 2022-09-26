using Microsoft.Win32;

using DayzServerTools.Application.Models;

namespace DayzServerTools.Windows.Models;

public class WindowsOpenFileDialog : IFileDialog
{
    private readonly OpenFileDialog _dialog;

    public string FileName
    {
        get => _dialog.FileName;
        set => _dialog.FileName = value;
    }
    public string Filter
    {
        get =>_dialog.Filter;
        set => _dialog.Filter=value;
    }
    public string Title
    {
        get => _dialog.Title;
        set => _dialog.Title = value;
    }
    public WindowsOpenFileDialog()
    {
        _dialog = new OpenFileDialog() { Multiselect = false };
    }

    public bool? ShowDialog() => _dialog.ShowDialog(); 
}
