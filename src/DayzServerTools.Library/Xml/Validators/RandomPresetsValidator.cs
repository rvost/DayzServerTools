using FluentValidation;

namespace DayzServerTools.Library.Xml.Validators;

public class RandomPresetsValidator : AbstractValidator<RandomPresets>
{
    public RandomPresetsValidator()
    {
        RuleFor(x => x.AttachmentsPresets)
            .NotEmpty()
            .When(x => x.CargoPresets.Count == 0)
            .WithMessage("cfgrandompresets.xml can not be empty");

        RuleFor(x => x.AttachmentsPresets.Concat(x.CargoPresets))
            .Must(NotContainDuplicates)
            .WithMessage("cfgrandompresets.xml contains repeating presets");
    }

    private bool NotContainDuplicates(IEnumerable<RandomPreset> items)
    {
        var duplicates = items.GroupBy(x => x.Name)
            .Where(g => g.Count() > 1);
        return !duplicates.Any();
    }
}
