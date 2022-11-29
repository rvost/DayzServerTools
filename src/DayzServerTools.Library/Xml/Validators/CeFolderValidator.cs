using FluentValidation;

namespace DayzServerTools.Library.Xml.Validators;

public class CeFolderValidator: AbstractValidator<CeFolder>
{

    private readonly string _missionPath;

    public CeFolderValidator(string missionPath)
    {
        _missionPath = missionPath;
        
        RuleFor(x => x.Folder)
            .Must(BeValidFolder)
            .WithMessage("Folder \"{PropertyValue}\" does not exists in mission folder");

        RuleForEach(x => x.Files)
            .SetValidator(FileValidatorFactory);
    }

    private bool BeValidFolder(string folder)
    {
        var fullPath = Path.Combine(_missionPath, folder);
        return Directory.Exists(fullPath);
    }

    private CeFileValidator FileValidatorFactory(CeFolder ceFolder)
    {
        var fileFolder = Path.Combine(_missionPath, ceFolder.Folder);
        return new CeFileValidator(fileFolder);
    }
}
