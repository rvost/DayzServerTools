using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace DayzServerTools.Library.Xml.Validation;

public static class UserDefinitionsValidation
{
    public static ValidationResult ValidateDefinitions(ObservableCollection<UserDefinableFlag> definitions,
        ValidationContext context)
    {
        var getAvailableDefinitions = (Func<IEnumerable<UserDefinableFlag>>)context.Items["definitions"];
        var availableDefinitions = getAvailableDefinitions();
        var validator = new UserFlagDefinitionValidator(availableDefinitions);
        return validator.Validate(definitions);
    }
}
