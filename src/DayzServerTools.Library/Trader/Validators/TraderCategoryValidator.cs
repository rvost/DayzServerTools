using FluentValidation;

namespace DayzServerTools.Library.Trader.Validators;

public class TraderCategoryValidator : AbstractValidator<TraderCategory>
{
    private readonly AbstractValidator<TraderItem> _itemValidator;

    public TraderCategoryValidator()
    {
        _itemValidator = new TraderItemValidator();

        RuleFor(x => x.CategoryName)
            .NotEmpty();

        RuleFor(x => x.TraderItems)
            .NotEmpty()
            .Must(NotContainDuplicates)
            .WithMessage(x => $"Category \"{x.CategoryName}\" contains repeating items");

        RuleForEach(x => x.TraderItems)
            .SetValidator(_itemValidator);
    }

    private bool NotContainDuplicates(IEnumerable<TraderItem> items)
    {
        var duplicates = items.GroupBy(x => x.Name)
            .Where(g => g.Count() > 1);
        return !duplicates.Any();
    }
}
