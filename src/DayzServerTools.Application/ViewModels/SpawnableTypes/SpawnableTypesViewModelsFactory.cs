using System;
using System.Collections.ObjectModel;

using DayzServerTools.Application.Services;
using DayzServerTools.Application.ViewModels.Base;
using DayzServerTools.Library.Common;
using DayzServerTools.Library.Xml;
using DayzServerTools.Library.Xml.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SpawnableTypesModel = DayzServerTools.Library.Xml.SpawnableTypes;

namespace DayzServerTools.Application.ViewModels.SpawnableTypes;

public class SpawnableTypesViewModelsFactory: IFileViewModelFactory<SpawnableTypesModel>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IDialogFactory _dialogFactory;
    private readonly IRandomPresetsProvider _randomPresetsProvider;

    public SpawnableTypesViewModelsFactory(IDialogFactory dialogFactory, IRandomPresetsProvider randomPresetsProvider,
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _dialogFactory = dialogFactory;
        _randomPresetsProvider = randomPresetsProvider;
    }

    public SpawnablePresetsCollectionProxy CreatePresetsCollectionProxy(PresetType type, string name,
        ObservableCollection<SpawnablePresetViewModel> presets) => new(type, name, presets, this);

    public SpawnableTypeViewModel CreateSpawnableTypeViewModel(SpawnableType model)
        => new(model, _dialogFactory,_randomPresetsProvider, this);

    public SpawnablePresetViewModel CreateSpawnablePresetViewModel(PresetType type, SpawnablePreset model)
    {
        SpawnablePresetValidator validator = type switch
        {
            PresetType.Attachments => new SpawnablePresetValidator(() => _randomPresetsProvider.AvailableAttachmentsPresets),
            PresetType.Cargo => new SpawnablePresetValidator(() => _randomPresetsProvider.AvailableCargoPresets),
        };

        return new(model, validator, _dialogFactory);
    }

    public ProjectFileViewModel<SpawnableTypesModel> Create(string filename, SpawnableTypesModel model)
    {
        var validator = _serviceProvider.GetRequiredService<IValidator<SpawnableTypesModel>>();
        return new SpawnableTypesViewModel(filename, model,validator, _dialogFactory, this);
    }
}
