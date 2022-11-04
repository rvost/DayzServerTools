using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.Input;

using DayzServerTools.Application.ViewModels.Base;
using DayzServerTools.Library.Xml;
using DayzServerTools.Library.Xml.Validators;

namespace DayzServerTools.Application.ViewModels.UserDefinitions;

public class UserDefinitionViewModel : ObservableFluentValidator<UserDefinition, UserDefinitionValidator>
{
    public string Name
    {
        get => _model.Name;
        set => SetProperty(_model.Name, value, _model, (m, v) => m.Name = v, true);
    }
    public ObservableCollection<UserDefinableFlag> Definitions { get => _model.Definitions; }

    public IRelayCommand<UserDefinableFlag> AddDefinitionCommand { get; }
    public IRelayCommand<UserDefinableFlag> RemoveDefinitionCommand { get; }

    public UserDefinitionViewModel(UserDefinition model, Func<IEnumerable<UserDefinableFlag>> getAvailableDefinitions)
        : base(model, new(getAvailableDefinitions))
    {
        AddDefinitionCommand = new RelayCommand<UserDefinableFlag>(
            flag => Definitions.Add(flag)
            );
        RemoveDefinitionCommand = new RelayCommand<UserDefinableFlag>(
            flag => Definitions.Remove(flag)
            );
    }
}
