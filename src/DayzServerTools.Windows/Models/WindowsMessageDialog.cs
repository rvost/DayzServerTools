using System;
using System.Windows;

using DayzServerTools.Application.Models;

namespace DayzServerTools.Windows.Models;

public class WindowsMessageDialog : IMessageDialog
{
    public string Message { get; set; }
    public string Title { get; set; }
    public MessageDialogImage Image { get; set; } = MessageDialogImage.None;

    public bool? Show()
    {
        var image = (MessageBoxImage)Enum.Parse(typeof(MessageBoxImage), Image.ToString());
        MessageBox.Show(Message, Title, MessageBoxButton.OK, image);
        return true;
    }
}
