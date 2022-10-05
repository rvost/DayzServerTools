using System.ComponentModel.DataAnnotations;

namespace DayzServerTools.Library.Xml.Validation;

public class UserFlagDefinitionValidator
{
    private HashSet<string> _acceptableValues;

    public UserFlagDefinitionValidator(IEnumerable<UserDefinableFlag> acceptableFlags)
    {
        _acceptableValues = new HashSet<string>(
            acceptableFlags.Select(flag => flag.Value)
        );
    }

    public ValidationResult Validate(ICollection<UserDefinableFlag> definition)
    {
        if (definition == null || _acceptableValues.Count == 0)
        {
            return ValidationResult.Success;
        }

        if (definition.Count == 0)
        {
            return new ValidationResult("User definition couldn't be empty");
        }

        var duplicates = definition.GroupBy(x => x.Value)
            .Where(g => g.Count() > 1)
            .Select(y => y.Key)
            .ToList();
        if (duplicates.Any())
        {
            return new ValidationResult($"User definition contains duplicates: {string.Join(',', duplicates)}");
        }

        var unknown = definition.ExceptBy(_acceptableValues, (flag) => flag.Value).ToList();
        if (unknown.Any())
        {
            return new ValidationResult($"User definition contains invalid values in context: {string.Join(',', unknown)}");
        }

        return ValidationResult.Success;
    }
}
