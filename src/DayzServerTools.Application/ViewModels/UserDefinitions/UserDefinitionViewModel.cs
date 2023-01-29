using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.Input;

using DayzServerTools.Application.ViewModels.Base;
using DayzServerTools.Library.Xml;
using DayzServerTools.Library.Xml.Validators;

namespace DayzServerTools.Application.ViewModels.UserDefinitions;

public partial class UserDefinitionViewModel : ObservableFluentValidator<UserDefinition, UserDefinitionValidator>
{
    public string Name
    {
        get => _model.Name;
        set => SetProperty(_model.Name, value, _model, (m, v) => m.Name = v, true);
    }
    public ObservableCollection<UserDefinableFlag> Definitions { get => _model.Definitions; }

    public UserDefinitionViewModel(UserDefinition model, Func<IEnumerable<UserDefinableFlag>> getAvailableDefinitions)
        : base(model, new(getAvailableDefinitions))
    {
    }

    [RelayCommand]
    private void AddDefinition(UserDefinableFlag flag)
        => Definitions.Add(flag);

    [RelayCommand]
    private void RemoveDefinition(UserDefinableFlag flag)
        => Definitions.Remove(flag);
}
