using DayzServerTools.Application.ViewModels.Base;

namespace DayzServerTools.Application.Messages;

public class ValidationErrorInfo
{
    public IProjectFileTab Sender { get; set; }
    public string Identifier { get; set; }
    public IEnumerable<string> Errors { get; set; }

    public ValidationErrorInfo(IProjectFileTab sender, string identifier, IEnumerable<string> errors)
    {
        Sender = sender;
        Identifier = identifier;
        Errors = errors;
    }
}
