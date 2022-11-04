using FluentValidation;

namespace DayzServerTools.Library.Xml.Validators;

public class UserDefinitionsValidator : AbstractValidator<UserDefinitions>
{
    public UserDefinitionsValidator()
    {
        RuleFor(x => x.UsageFlags)
            .NotEmpty()
            .When(x => x.ValueFlags.Count == 0)
            .WithMessage("User definitions can not be empty");
    }
}