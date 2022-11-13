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

        RuleFor(x => x.UsageFlags.Concat(x.ValueFlags))
            .Must(NotContainDuplicates)
            .WithMessage("cfglimitsdefinitionuser.xml contains repeating flags definitions");
    }

    private bool NotContainDuplicates(IEnumerable<UserDefinition> items)
    {
        var duplicates = items.GroupBy(x => x.Name)
            .Where(g => g.Count() > 1);
        return !duplicates.Any();
    }
}