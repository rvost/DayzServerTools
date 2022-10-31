using FluentValidation;

namespace DayzServerTools.Library.Xml.Validators;

public class ItemTypesValidator:AbstractValidator<ItemTypes>
{
	public ItemTypesValidator()
	{
		RuleFor(x => x.Types)
			.NotEmpty()
			.WithMessage("types.xml can not be empty");

		//RuleForEach(x => x.Types)
		//	.SetValidator(new ItemTypeValidator());
	}
}
