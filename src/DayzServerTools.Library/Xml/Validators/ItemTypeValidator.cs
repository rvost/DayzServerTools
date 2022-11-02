using DayzServerTools.Library.Common;
using FluentValidation;

namespace DayzServerTools.Library.Xml.Validators;

public class ItemTypeValidator : AbstractValidator<ItemType>
{
    const int MAX_LIFETIME = 3888000; // 45 days
    const int MAX_RESTOCK = 3888000;

    private readonly ILimitsDefinitionsProvider _limitsDefinitions;

    public ItemTypeValidator(ILimitsDefinitionsProvider limitsDefinitions)
    {
        _limitsDefinitions = limitsDefinitions;

        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Matches(@"^\w+$");

        RuleFor(x => x.Min)
            .GreaterThanOrEqualTo(0);
        RuleFor(x => x.Nominal)
            .GreaterThanOrEqualTo(0)
            .GreaterThanOrEqualTo(x => x.Min);

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

        RuleFor(x => x.Category)
            .Must(BeValidCategory)
            .WithMessage("The category name \"{PropertyValue}\" is invalid for this mission");

        RuleForEach(x => x.Value)
            .Must(BeValidValueFlag)
            .WithMessage("The value flag \"{PropertyValue}\" is invalid for this mission");
        RuleFor(x => x.Value)
            .Cascade(CascadeMode.Stop)
            .Must(NotContainMixedDefinitions)
            .WithMessage("Mixed flags not supported")
            .Must(NotContainMultipleUserFlags)
            .WithMessage("Multiple user-defined flags not supported");

        RuleForEach(x => x.Usages)
            .Must(BeValidUsageFlag)
            .WithMessage("The usage flag \"{PropertyValue}\" is invalid for this mission");
        RuleFor(x => x.Usages)
            .Cascade(CascadeMode.Stop)
            .Must(NotContainMixedDefinitions)
            .WithMessage("Mixed flags not supported")
            .Must(NotContainMultipleUserFlags)
            .WithMessage("Multiple user-defined flags not supported");

        RuleForEach(x => x.Tags)
            .Must(BeValidTag)
            .WithMessage("The tag \"{PropertyValue}\" is invalid for this mission");
    }

    private bool BeValidCategory(VanillaFlag category)
    {
        if (category == null || _limitsDefinitions.Categories.Count == 0)
        {
            return true;
        }
        else
        {
            return _limitsDefinitions.Categories.Contains(category);
        }
    }

    private bool BeValidValueFlag(UserDefinableFlag value)
    {
        if (_limitsDefinitions.Values.Count == 0)
        {
            return true;
        }
        else
        {
            return _limitsDefinitions.Values.Contains(value);
        }
    }

    private bool BeValidUsageFlag(UserDefinableFlag usage)
    {
        if (_limitsDefinitions.Usages.Count == 0)
        {
            return true;
        }
        else
        {
            return _limitsDefinitions.Usages.Contains(usage);
        }
    }

    private bool BeValidTag(VanillaFlag tag)
    {
        if (_limitsDefinitions.Tags.Count == 0)
        {
            return true;
        }
        else
        {
            return _limitsDefinitions.Tags.Contains(tag);
        }
    }

    private bool NotContainMixedDefinitions(IEnumerable<UserDefinableFlag> flags)
    {
        var hasVanillaFlags = flags.Any(flag => flag.DefinitionType == FlagDefinition.Vanilla);
        var hasUserFlags = flags.Any(flag => flag.DefinitionType == FlagDefinition.User);

        if (hasVanillaFlags && hasUserFlags)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    private bool NotContainMultipleUserFlags(IEnumerable<UserDefinableFlag> flags)
    {
        var userFlagsCount = flags.Where(flag => flag.DefinitionType == FlagDefinition.User).Count();
        if (userFlagsCount > 1)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
