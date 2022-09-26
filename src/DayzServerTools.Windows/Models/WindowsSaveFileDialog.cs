using Microsoft.Win32;

using DayzServerTools.Application.Models;

namespace DayzServerTools.Windows.Models;

public class WindowsSaveFileDialog: IFileDialog
{
    private readonly SaveFileDialog _dialog;

    public string FileName
    {
        get => _dialog.FileName;
        set => _dialog.FileName = value;
    }
    public string Filter
    {
        get => _dialog.Filter;
        set => _dialog.Filter = value;
    }
    public string Title
    {
        get => _dialog.Title;
        set => _dialog.Title = value;
    }

    public WindowsSaveFileDialog()
    {
        _dialog = new SaveFileDialog();
    }

    public bool? ShowDialog() => _dialog.ShowDialog();
}
