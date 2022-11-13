using FluentValidation;

namespace DayzServerTools.Library.Xml.Validators;

public class ItemTypesValidator:AbstractValidator<ItemTypes>
{
	public ItemTypesValidator()
	{
		RuleFor(x => x.Types)
			.NotEmpty()
			.WithMessage("types.xml can not be empty")
            .Must(NotContainDuplicates)
            .WithMessage("types.xml contains repeating items");
	}

    private bool NotContainDuplicates(IEnumerable<ItemType> items)
    {
        var duplicates = items.GroupBy(x => x.Name)
            .Where(g => g.Count() > 1);
        return !duplicates.Any();
    }
}
