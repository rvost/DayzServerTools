using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using DayzServerTools.Library.Xml;
using DayzServerTools.Library.Xml.Validation;

namespace DayzServerTools.Application.ViewModels;

public class UserDefinitionViewModel : ObservableValidator
{
    private UserDefinition _model;

    public UserDefinition Model { get => _model; }
    [MinLength(1)]
    public string Name
    {
        get => _model.Name;
        set => SetProperty(_model.Name, value, _model, (m, v) => m.Name = v, true);
    }
    [CustomValidation(typeof(UserDefinitionViewModel), nameof(ValidateDefinitions))]
    public ObservableCollection<UserDefinableFlag> Definitions { get => _model.Definitions; }

    public IRelayCommand<UserDefinableFlag> AddDefinitionCommand { get; }
    public IRelayCommand<UserDefinableFlag> RemoveDefinitionCommand { get; }

    public UserDefinitionViewModel(UserDefinition model, IEnumerable<UserDefinableFlag> availableDefinitions)
        : base(new Dictionary<object, object>() { { "definitions", availableDefinitions } })
    {
        _model = model;

        AddDefinitionCommand = new RelayCommand<UserDefinableFlag>(
            flag => Definitions.Add(flag)
            );
        RemoveDefinitionCommand = new RelayCommand<UserDefinableFlag>(
            flag => Definitions.Remove(flag)
            );
    }

    public void ValidateSelf() => ValidateAllProperties();
    public static ValidationResult ValidateDefinitions(ObservableCollection<UserDefinableFlag> definitions, ValidationContext context)
    {
        var availableDefinitions = (IEnumerable<UserDefinableFlag>)context.Items["definitions"] ?? Enumerable.Empty<UserDefinableFlag>();
        var validator = new UserFlagDefinitionValidator(availableDefinitions);
        return validator.Validate(definitions);
    }
}
