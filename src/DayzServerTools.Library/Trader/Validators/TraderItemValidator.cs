using FluentValidation;

namespace DayzServerTools.Library.Trader.Validators;

public class TraderItemValidator : AbstractValidator<TraderItem>
{
    public TraderItemValidator()
    {
        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Matches(@"^\w+$");

        RuleFor(x => x.Modifier)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Matches(@"^(\*|W|M|V|VNK|K|S|[1-9]\d*)$")
            .WithMessage("Invalid Quantity Modifier \"{PropertyValue}\"");

        RuleFor(x => x.BuyPrice)
            .Cascade(CascadeMode.Stop)
            .GreaterThanOrEqualTo(-1)
            .WithMessage("Buy price must be greater than or equal to -1")
            .GreaterThan(-1)
            .When(x => x.SellPrice == -1)
            .WithMessage(x => $"Item \"{x.Name}\" can not be sold or purchased");

        RuleFor(x => x.SellPrice)
           .Cascade(CascadeMode.Stop)
           .GreaterThanOrEqualTo(-1)
           .WithMessage("Sell price must be greater than or equal to -1");
    }
}