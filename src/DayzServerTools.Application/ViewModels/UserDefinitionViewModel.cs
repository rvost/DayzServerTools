using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using DayzServerTools.Library.Xml;

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
    public ObservableCollection<UserDefinableFlag> Definitions { get => _model.Definitions; }

    public IRelayCommand<UserDefinableFlag> AddDefinitionCommand { get; }
    public IRelayCommand<UserDefinableFlag> RemoveDefinitionCommand { get; }

    public UserDefinitionViewModel(UserDefinition model)
    {
        _model = model;

        AddDefinitionCommand = new RelayCommand<UserDefinableFlag>(
            flag => Definitions.Add(flag)
            );
        RemoveDefinitionCommand = new RelayCommand<UserDefinableFlag>(
            flag => Definitions.Remove(flag)
            );
    }
}
