using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using DayzServerTools.Application.Messages;
using DayzServerTools.Application.Services;

namespace DayzServerTools.Application.ViewModels.Panes;

public partial class ErrorsPaneViewModel : ObservableRecipient, IPane, 
    IRecipient<ValidationErrorInfo>, IRecipient<ClearValidationErrorsMessage>
{
    private readonly IDispatcherService _dispatcher;

    [ObservableProperty]
    private bool isVisible = true;
    [ObservableProperty]
    private bool isSelected = false;

    public string Title => "Validation Errors";

    public ObservableCollection<ValidationErrorInfo> Errors { get; } = new();

    public ErrorsPaneViewModel(IDispatcherService dispatcher)
    {
        _dispatcher = dispatcher;
        IsActive = true;
    }

    public void Receive(ValidationErrorInfo message)
    {
        _dispatcher.BeginInvoke(() => Errors.Add(message));
    }

    public void Receive(ClearValidationErrorsMessage message)
    {
         _dispatcher.BeginInvoke(() =>
        {
            var errors = Errors.Where(e => e.Sender == message.Sender).ToList();
            foreach (var error in errors)
            {
                Errors.Remove(error);
            }
        });
        IsVisible = true;
    }
}
