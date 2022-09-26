using System.ComponentModel.DataAnnotations;

namespace DayzServerTools.Library.Xml.Validation;

public class UserFlagValidator
{
    private HashSet<(FlagDefinition, string)> _acceptableValues;

    public UserFlagValidator(IEnumerable<UserDefinableFlag> acceptableFlags)
    {
        _acceptableValues = new HashSet<(FlagDefinition, string)>(
            acceptableFlags.Select(flag => (flag.DefinitionType, flag.Value))
        );
    }

    public ValidationResult Validate(UserDefinableFlag flag)
    {
        if (flag == null || _acceptableValues.Count == 0)
        {
            return ValidationResult.Success;
        }

        if (string.IsNullOrEmpty(flag.Value))
        {
            return flag.DefinitionType == FlagDefinition.Vanilla ?
                ValidationResult.Success : new ValidationResult("User defined flag shouldn't be empty");
        }
        else
        {
            return _acceptableValues.Contains((flag.DefinitionType, flag.Value)) ?
                ValidationResult.Success : new ValidationResult("Unacceptable value in context");
        }
    }
}
