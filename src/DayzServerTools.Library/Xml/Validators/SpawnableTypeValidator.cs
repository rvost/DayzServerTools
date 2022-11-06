using DayzServerTools.Library.Common;
using FluentValidation;

namespace DayzServerTools.Library.Xml.Validators;

public class SpawnableTypeValidator : AbstractValidator<SpawnableType>
{
    private readonly IRandomPresetsProvider _randomPresets;
    private readonly SpawnablePresetValidator _cargoPresetValidator;
    private readonly SpawnablePresetValidator _attachmentsPresetValidator;

    public SpawnableTypeValidator(IRandomPresetsProvider randomPresets)
    {
        _randomPresets = randomPresets;

        _cargoPresetValidator = new SpawnablePresetValidator(() => _randomPresets.AvailableCargoPresets);
        _attachmentsPresetValidator = new SpawnablePresetValidator(() => _randomPresets.AvailableAttachmentsPresets);

        RuleFor(x => x.Name)
           .Cascade(CascadeMode.Stop)
           .NotEmpty()
           .Matches(@"^\w+$");

        When(x => x.DamageSpecified, () =>
        {
            RuleFor(x => x.Damage.Min)
                .Cascade(CascadeMode.Stop)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Minimal Damage must be greater than or equal to 0")
                .LessThanOrEqualTo(x => x.Damage.Max)
                .WithMessage("Minimal Damage must be less than or equal to Maximum Damage");

            RuleFor(x => x.Damage.Max)
                .LessThan(1)
                .WithMessage("Maximum Damage must be less than or equal to 1");
        });

        When(x => x.HoarderSpecified, () =>
        {
            RuleFor(x => x.Attachments)
                .Must(x => x.Count == 0)
                .WithMessage("Hoarder can not have any attachments");
            RuleFor(x => x.Cargo)
                .Must(x => x.Count == 0)
                .WithMessage("Hoarder can not have any cargo");
        }).Otherwise(() =>
        {
            RuleForEach(x => x.Attachments)
                .SetValidator(_attachmentsPresetValidator);
            RuleForEach(x => x.Cargo)
                .SetValidator(_cargoPresetValidator);
        });
    }
}
