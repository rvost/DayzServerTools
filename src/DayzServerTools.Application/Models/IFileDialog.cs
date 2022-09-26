namespace DayzServerTools.Application.Models;

public interface IFileDialog
{
    string FileName { get; set; }
    string Filter { get; set; }
    string Title { get; set; }
   
    bool? ShowDialog();
}
