using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FluentValidation;

using DayzServerTools.Application.Extensions;
using DayzServerTools.Application.Messages;
using DayzServerTools.Application.Services;
using DayzServerTools.Application.ViewModels.Base;
using DayzServerTools.Library.Xml;
using SpawnableTypesModel = DayzServerTools.Library.Xml.SpawnableTypes;

namespace DayzServerTools.Application.ViewModels.SpawnableTypes;

public partial class SpawnableTypesViewModel : ProjectFileViewModel<SpawnableTypesModel>,
    IImporter<IEnumerable<string>>, IDisposable
{
    private readonly SpawnableTypesViewModelsFactory _viewModelsFactory;

    [ObservableProperty]
    private SpawnableTypeViewModel selectedItem;
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ExportToNewFileCommand),
        nameof(SetMinDamageCommand), nameof(SetMaxDamageCommand))]
    private IList selectedItems;
    [ObservableProperty]
    private bool showTagColumn = false;

    public ObservableCollection<SpawnableTypeViewModel> Spawnables { get; set; } = new();

    public SpawnableTypesViewModel(string fileName, SpawnableTypesModel model, IValidator<SpawnableTypesModel> validator,
        IDialogFactory dialogFactory, SpawnableTypesViewModelsFactory viewModelsFactory) : base(fileName, model, validator, dialogFactory)
    {
        _viewModelsFactory = viewModelsFactory;
        
        Spawnables.AddRange(
            model.Spawnables.Select(type => _viewModelsFactory.CreateSpawnableTypeViewModel(type))
            );

        Spawnables.CollectionChanged += OnSpawnablesCollectionChanged;
    }

    public void CopySpawnableTypes(IEnumerable<SpawnableType> source)
    {
        Spawnables.AddRange(
            source.Select(obj => _viewModelsFactory.CreateSpawnableTypeViewModel(obj.Copy()))
        );
    }
    public void Import(IEnumerable<string> classnames)
    {
        var models = classnames.Select(name => new SpawnableType() { Name = name });
        var viewModels = models.Select(models => _viewModelsFactory.CreateSpawnableTypeViewModel(models));
        Spawnables.AddRange(viewModels);
    }

    private bool CanExecuteBatchCommand() => SelectedItems is not null;
    private bool CanExecuteExportCommand(object cmdParam)
        => cmdParam is not null || SelectedItems is not null;

    [RelayCommand]
    private void AddSpawnableType()
        => Spawnables.Add(_viewModelsFactory.CreateSpawnableTypeViewModel(new()));

    [RelayCommand(CanExecute =nameof(CanExecuteExportCommand))]
    private void ExportToNewFile(object cmdParam)
    {
        var list = (IList)cmdParam ?? SelectedItems;
        var viewModels = list.Cast<SpawnableTypeViewModel>();

        var items = viewModels.Select(vm => vm.Model);
        var newVM = (SpawnableTypesViewModel)_viewModelsFactory.Create("cfgspawnabletypes.xml", new());
        newVM.CopySpawnableTypes(items);
        // TODO: Inject Messenger
        WeakReferenceMessenger.Default.Send<IProjectFileTab>(newVM);
    }

    [RelayCommand(CanExecute =nameof(CanExecuteBatchCommand))]
    private void SetMinDamage(double value)
    {
        if (SelectedItems is null)
        {
            return;
        }
        var viewModels = SelectedItems.Cast<SpawnableTypeViewModel>();
        viewModels.AsParallel().ForAll(spawnable => spawnable.MinDamage = value);
    }

    [RelayCommand(CanExecute = nameof(CanExecuteBatchCommand))]
    private void SetMaxDamage(double value)
    {
        if (SelectedItems is null)
        {
            return;
        }
        var viewModels = SelectedItems.Cast<SpawnableTypeViewModel>();
        viewModels.AsParallel().ForAll(spawnable => spawnable.MaxDamage = value);
    }

    protected override bool Validate()
    {
        WeakReferenceMessenger.Default.Send(new ClearValidationErrorsMessage(this));

        var itemsErrors = Spawnables.AsParallel()
            .Select(item => new { item.Name, Result = item.ValidateSelf() })
            .Where(x => !x.Result.IsValid)
            .Select(x => new ValidationErrorInfo(this, x.Name, x.Result.Errors.Select(x => x.ErrorMessage)))
            .ToList();

        bool itemsHaveErrors = itemsErrors.Any();

        if (itemsHaveErrors)
        {
            itemsErrors.AsParallel().ForAll(error => WeakReferenceMessenger.Default.Send(error));
        }

        var res = _validator.Validate(Model);
        if (!res.IsValid)
        {
            res.Errors.AsParallel()
                .Select(error => new ValidationErrorInfo(this, "", new[] { error.ErrorMessage }))
                .ForAll(error => WeakReferenceMessenger.Default.Send(error));
        }

        return res.IsValid && !itemsHaveErrors;
    }

    private void OnSpawnablesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                foreach (var item in e.NewItems)
                {
                    Model.Spawnables.Add(((SpawnableTypeViewModel)item).Model);
                }
                break;
            case NotifyCollectionChangedAction.Remove:
                foreach (var item in e.OldItems)
                {
                    Model.Spawnables.Remove(((SpawnableTypeViewModel)item).Model);
                }
                break;
            default:
                break;
        }
    }

    public void Dispose()
    {
        Spawnables.CollectionChanged -= OnSpawnablesCollectionChanged;
    }
}
