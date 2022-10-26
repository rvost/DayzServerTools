using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DayzServerTools.Library.Xml.Validation;

public static class ItemTypesValidation
{
    public static ValidationResult ValidateCategory(VanillaFlag category, ValidationContext context)
    {
        var getCategories = (Func<IEnumerable<VanillaFlag>>)context.Items["categories"];
        var validator = new VanillaFalgValidator(getCategories());

        var result = validator.Validate(category);
        return result == ValidationResult.Success ? result : new ValidationResult($"Category: {result.ErrorMessage}");
    }

    public static ValidationResult ValidateUsages(ObservableCollection<UserDefinableFlag> usages, ValidationContext context)
    {
        var getUsages = (Func<IEnumerable<UserDefinableFlag>>)context.Items["usages"];
        var validator = new UserFlagValidator(getUsages());

        if (usages.Where(usage => usage.DefinitionType == FlagDefinition.User).Count() > 1)
        {
            return new ValidationResult($"Usage Flags:\n\tOnly multiple user flags not allowed");
        }

        if (usages.Any(usage => usage.DefinitionType == FlagDefinition.User) &&
            usages.Any(usage => usage.DefinitionType == FlagDefinition.Vanilla))
        {
            return new ValidationResult($"Usage Flags:\n\tMixed flag definitions not allowed");
        }

        var errorsBuilder = new StringBuilder();
        ValidationResult itemResult;
        foreach (var usage in usages)
        {
            itemResult = validator.Validate(usage);
            if (itemResult != ValidationResult.Success)
            {
                errorsBuilder.AppendLine($"\t{usage.ToString()}: {itemResult.ErrorMessage}");
            }
        }
        var errors = errorsBuilder.ToString();

        if (string.IsNullOrEmpty(errors))
        {
            return ValidationResult.Success;
        }
        else
        {
            return new ValidationResult($"Usage Flags:\n{errors}");
        }
    }
    public static ValidationResult ValidateValues(ObservableCollection<UserDefinableFlag> values, ValidationContext context)
    {
        var getValues = (Func<IEnumerable<UserDefinableFlag>>)context.Items["values"];
        var validator = new UserFlagValidator(getValues());

        if (values.Where(value => value.DefinitionType == FlagDefinition.User).Count() > 1)
        {
            return new ValidationResult($"Value Flags:\n\tOnly multiple user flags not allowed");
        }

        if (values.Any(value => value.DefinitionType == FlagDefinition.User) &&
            values.Any(value => value.DefinitionType == FlagDefinition.Vanilla))
        {
            return new ValidationResult($"Value Flags:\n\tMixed flag definitions not allowed");
        }

        var errorsBuilder = new StringBuilder();
        ValidationResult itemResult;
        foreach (var value in values)
        {
            itemResult = validator.Validate(value);
            if (itemResult != ValidationResult.Success)
            {
                errorsBuilder.AppendLine($"\t{value.ToString()}: {itemResult.ErrorMessage}");
            }
        }
        var errors = errorsBuilder.ToString();

        if (string.IsNullOrEmpty(errors))
        {
            return ValidationResult.Success;
        }
        else
        {
            return new ValidationResult($"Value Flags:\n{errors}");
        }
    }
    public static ValidationResult ValidateTags(ObservableCollection<VanillaFlag> tags, ValidationContext context)
    {
        var getTags = (Func<IEnumerable<VanillaFlag>>)context.Items["tags"];
        var validator = new VanillaFalgValidator(getTags());

        var errorsBuilder = new StringBuilder();
        ValidationResult tagResult;
        foreach (var tag in tags)
        {
            tagResult = validator.Validate(tag);
            if (tagResult != ValidationResult.Success)
            {
                errorsBuilder.AppendLine($"\tTag {tag.Value}: {tagResult.ErrorMessage}");
            }
        }
        var errors = errorsBuilder.ToString();

        if (string.IsNullOrEmpty(errors))
        {
            return ValidationResult.Success;
        }
        else
        {
            return new ValidationResult($"Tags:\n{errors}");
        }
    }
}
