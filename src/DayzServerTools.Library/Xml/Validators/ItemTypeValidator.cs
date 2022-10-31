using FluentValidation;

namespace DayzServerTools.Library.Xml.Validators;

public class ItemTypeValidator: AbstractValidator<ItemType>
{
	const int MAX_LIFETIME = 3888000;
	const int MAX_RESTOCK = 3888000;

    public ItemTypeValidator()
	{
		RuleFor(x => x.Name)
			.NotEmpty()
			.Matches(@"^\w+$");
        
		RuleFor(x => x.Min)
            .GreaterThanOrEqualTo(0);
        RuleFor(x => x.Nominal)
			.GreaterThanOrEqualTo(0)
			.GreaterThanOrEqualTo(x =>x.Min);
		
		RuleFor(x => x.Lifetime)
			.InclusiveBetween(0, MAX_LIFETIME);
		RuleFor(x => x.Restock)
            .InclusiveBetween(0, MAX_RESTOCK);

        RuleFor(x => x.Quantmin)
			.InclusiveBetween(-1, 100);
		RuleFor(x => x.Quantmax)
			.InclusiveBetween(-1, 100)
			.GreaterThanOrEqualTo(x => x.Quantmin);
		
		RuleFor(x => x.Cost)
			.InclusiveBetween(0, 100);
    }
}
