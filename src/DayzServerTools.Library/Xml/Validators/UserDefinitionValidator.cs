using FluentValidation;

namespace DayzServerTools.Library.Xml.Validators;

public class UserDefinitionValidator : AbstractValidator<UserDefinition>
{
    private Func<IEnumerable<UserDefinableFlag>> _getAcceptableFlags;

    public UserDefinitionValidator(Func<IEnumerable<UserDefinableFlag>> getAcceptableFlags)
    {
        _getAcceptableFlags = getAcceptableFlags;

        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Matches(@"^\w+$");

        RuleForEach(x => x.Definitions)
            .Must(BeValidFlag)
            .WithMessage("Flag \"{PropertyValue}\" is invalid for this mission");

        RuleFor(x => x.Definitions)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Must(NotContainDuplicates)
            .WithMessage("{PropertyName} can not be empty");
    }

    private bool BeValidFlag(UserDefinableFlag flag)
    {
        var acceptableFlags = _getAcceptableFlags();
        if (acceptableFlags.Any())
        {
            return acceptableFlags.Contains(flag);
        }
        else
        {
            return true;
        }
    }

    private bool NotContainDuplicates(IEnumerable<UserDefinableFlag> flags)
    {
        var acceptableFlags = _getAcceptableFlags();
        if (acceptableFlags.Any())
        {
            var duplicates = flags.GroupBy(x => x.Value)
               .Where(g => g.Count() > 1);
            return !duplicates.Any();
        }
        else
        {
            return true;
        }
    }
}
