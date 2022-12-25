namespace DayzServerTools.Application.Models;

public interface IFolderDialog
{
    string FileName { get; }
    string Title { get; set; }

    bool? ShowDialog();
}
