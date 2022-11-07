using FluentValidation;

namespace DayzServerTools.Library.Trader.Validators;

public class TraderConfigValidator : AbstractValidator<TraderConfig>
{
    public TraderConfigValidator()
    {
        RuleFor(x => x.Traders)
            .NotEmpty();
    }
}
