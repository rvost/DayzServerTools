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
    }
}
