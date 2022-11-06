using FluentValidation;

namespace DayzServerTools.Library.Xml.Validators;

public class SpawnablePresetValidator : AbstractValidator<SpawnablePreset>
{
    private readonly Func<IEnumerable<string>> _getAcceptablePresets;
    private readonly SpawnableItemValidator _itemValidator;
    public SpawnablePresetValidator(Func<IEnumerable<string>> getAcceptablePresets)
    {
        _getAcceptablePresets = getAcceptablePresets;
        _itemValidator = new SpawnableItemValidator();

        When(x => x.PresetSpecified, () =>
        {
            RuleFor(x => x.Preset)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .Must(BeValidPresetName)
                .WithMessage("The random preset \"{PropertyValue}\"  is invalid for this mission");
        }).Otherwise(() =>
        {
            RuleFor(x => x.Chance)
                .Must(ValidationHelpers.BeCorrectProbabilityValue)
                .When(x => x.ChanceSpecified)
                .WithMessage("{PropertyName} must be greater than or equal to 0 and less than or equal to 1");

            RuleFor(x => x.Items)
                .NotEmpty();

            RuleForEach(x => x.Items)
                .SetValidator(_itemValidator);
        });
    }

    private bool BeValidPresetName(string preset)
    {
        var acceptablePresets = _getAcceptablePresets();

        if (acceptablePresets.Any())
        {
            return acceptablePresets.Contains(preset);
        }
        else
        {
            return true;
        }
    }
}