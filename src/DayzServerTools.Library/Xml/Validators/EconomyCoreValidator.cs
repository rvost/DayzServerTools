using FluentValidation;

namespace DayzServerTools.Library.Xml.Validators;

public class EconomyCoreValidator: AbstractValidator<EconomyCore>
{
    private readonly CeFolderValidator _folderValidator;

    public EconomyCoreValidator(string missionPath)
    {
        _folderValidator = new CeFolderValidator(missionPath);

        RuleForEach(x => x.CeFolders)
            .SetValidator(_folderValidator);
    }
}
