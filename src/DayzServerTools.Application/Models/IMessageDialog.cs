namespace DayzServerTools.Application.Models;

public enum MessageDialogImage
{
    None,
    Error,
    Warning,
    Information
}

public interface IMessageDialog
{
    string Message { get; set; }
    string Title { get; set; }
    MessageDialogImage Image { get; set; }

    bool? Show();
}
