using FluentValidation;

namespace DayzServerTools.Library.Trader.Validators;

public class TraderValidator : AbstractValidator<Trader>
{
    private readonly AbstractValidator<TraderCategory> _categoryValidator;

    public TraderValidator()
    {
        _categoryValidator = new TraderCategoryValidator();

        RuleFor(x => x.TraderName)
            .NotEmpty();
        RuleFor(x => x.TraderCategories)
            .NotEmpty()
            .WithMessage("Trader must have at least 1 category");

        RuleForEach(x => x.TraderCategories)
            .SetValidator(_categoryValidator);
    }
}
