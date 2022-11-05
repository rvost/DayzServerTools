using FluentValidation;

namespace DayzServerTools.Library.Xml.Validators;

public class SpawnableItemValidator : AbstractValidator<SpawnableItem>
{
    public SpawnableItemValidator()
    {
        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Matches(@"^\w+$")
            .WithName("Item \"{PropertyValue}\" has an invalid name format");

        RuleFor(x => x.Chance)
          .Must(ValidationHelpers.BeCorrectProbabilityValue)
          .When(x => !double.IsNaN(x.Chance))
          .WithMessage(x => $"Item \"{x.Name}\" chance must be greater than or equal to 0 and less than or equal to 1");
    }
}
