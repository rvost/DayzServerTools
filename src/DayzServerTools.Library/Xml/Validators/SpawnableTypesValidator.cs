using FluentValidation;

namespace DayzServerTools.Library.Xml.Validators;

public class SpawnableTypesValidator : AbstractValidator<SpawnableTypes>
{
    public SpawnableTypesValidator()
    {
        RuleFor(x => x.Spawnables)
            .NotEmpty()
            .WithMessage("cfgspawnabletypes.xml can not be empty");
    }
}
