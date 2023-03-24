namespace DayzServerTools.Application.Models;

public enum ConfirmationDialogResult
{
    None = 0,
    OK = 1,
    Cancel = 2,
    Yes = 6,
    No = 7
}

public enum ConfirmationDialogButton
{
    OK = 0,
    OKCancel = 1,
    YesNoCancel = 3,
    YesNo = 4
}

public interface IConfirmationDialog
{
    string Message { get; set; }
    string Title { get; set; }
    MessageDialogImage Image { get; set; }
    ConfirmationDialogButton Button { get; set; }

    ConfirmationDialogResult Show();
}