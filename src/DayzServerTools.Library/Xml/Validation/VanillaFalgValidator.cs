using System.ComponentModel.DataAnnotations;

namespace DayzServerTools.Library.Xml.Validation;

public class VanillaFalgValidator
{
    private HashSet<string> _acceptableValues;

    public VanillaFalgValidator(IEnumerable<VanillaFlag> acceptableFlags)
    {
        _acceptableValues = new HashSet<string>(
            acceptableFlags.Where(f => !string.IsNullOrEmpty(f.Value)).Select(f => f.Value)
            );
    }

    public ValidationResult Validate(VanillaFlag flag)
    {
        if (string.IsNullOrEmpty(flag?.Value) || _acceptableValues.Count == 0)
        {
            return ValidationResult.Success;
        }
        else
        {
            return _acceptableValues.Contains(flag.Value) ?
                        ValidationResult.Success : new ValidationResult("Unacceptable value in context");
        }
    }
}
