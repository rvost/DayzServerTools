using System;
using System.Windows;

using DayzServerTools.Application.Models;

namespace DayzServerTools.Windows.Models;

public class WindowsConfirmationDialog : IConfirmationDialog
{
    public string Message { get; set; }
    public string Title { get; set; }
    public MessageDialogImage Image { get; set; } = MessageDialogImage.None;
    public ConfirmationDialogButton Button { get; set; } = ConfirmationDialogButton.YesNo;

    public ConfirmationDialogResult Show()
    {
        var button = (MessageBoxButton)Enum.Parse(typeof(MessageBoxButton), Button.ToString());
        var image = (MessageBoxImage)Enum.Parse(typeof(MessageBoxImage), Image.ToString());

        var res = MessageBox.Show(Message, Title, button, image);
        return (ConfirmationDialogResult)Enum.Parse(typeof(ConfirmationDialogResult), res.ToString());
    }
}