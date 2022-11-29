using FluentValidation;

namespace DayzServerTools.Library.Xml.Validators;

public class CeFileValidator : AbstractValidator<CeFile>
{
    private readonly string _fileFolder;

    public CeFileValidator(string fileFolder)
    {
        _fileFolder = fileFolder;

        RuleFor(x => x.Name)
            .Must(Exists)
            .WithMessage("File \"{PropertyValue}\" does not exists in mission folder");
    }

    private bool Exists(string name)
    {
        var fullPath = Path.Combine(_fileFolder, name);
        return File.Exists(fullPath);
    }
}