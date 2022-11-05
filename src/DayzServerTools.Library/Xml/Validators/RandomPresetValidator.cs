using FluentValidation;

namespace DayzServerTools.Library.Xml.Validators;

public class RandomPresetValidator : AbstractValidator<RandomPreset>
{
    private readonly SpawnableItemValidator _itemValidator = new();
    public RandomPresetValidator()
    {
        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Matches(@"^\w+$");

        RuleFor(x => x.Chance)
            .Must(ValidationHelpers.BeCorrectProbabilityValue)
            .When(x => !double.IsNaN(x.Chance))
            .WithMessage("{PropertyName} must be greater than or equal to 0 and less than or equal to 1");

        RuleFor(x => x.Items)
            .NotEmpty();
        RuleForEach(x => x.Items)
            .SetValidator(_itemValidator);
    }


}
