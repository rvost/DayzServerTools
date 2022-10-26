using System.ComponentModel.DataAnnotations;

namespace DayzServerTools.Library.Xml.Validation;

public static class SpawnableTypesValidation
{
    public static ValidationResult ValidateChance(double value, ValidationContext context)
    {
        if (!double.IsNaN(value))
        {
            if (value >= 0 && value <= 1)
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult("The value must be greater than or equal to 0 and less than or equal to 1");
            }
        }
        return ValidationResult.Success;
    }
}
