using DayzServerTools.Application.ViewModels.Base;

namespace DayzServerTools.Application.Messages;

public class ClearValidationErrorsMessage
{
    public IProjectFileTab Sender { get; }

	public ClearValidationErrorsMessage(IProjectFileTab sender)
	{
		Sender = sender;
	}
}
