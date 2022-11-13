using FluentValidation;

namespace DayzServerTools.Library.Xml.Validators;

public class SpawnableTypesValidator : AbstractValidator<SpawnableTypes>
{
    public SpawnableTypesValidator()
    {
        RuleFor(x => x.Spawnables)
            .NotEmpty()
            .WithMessage("cfgspawnabletypes.xml can not be empty")
            .Must(NotContainDuplicates)
            .WithMessage("cfgspawnabletypes.xml contains repeating items");
    }

    private bool NotContainDuplicates(IEnumerable<SpawnableType> items)
    {
        var duplicates = items.GroupBy(x => x.Name)
            .Where(g => g.Count() > 1);
        return !duplicates.Any();
    }
}
