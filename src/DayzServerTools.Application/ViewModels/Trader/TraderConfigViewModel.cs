using System.Collections.ObjectModel;
using System.Collections.Specialized;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using FluentValidation;

using DayzServerTools.Application.ViewModels.Base;
using DayzServerTools.Application.Services;
using DayzServerTools.Application.Extensions;
using DayzServerTools.Application.Messages;
using DayzServerTools.Library.Trader;

namespace DayzServerTools.Application.ViewModels.Trader;

public partial class TraderConfigViewModel : ProjectFileViewModel<TraderConfig>, IDisposable
{
    private readonly TraderViewModelsFactory _viewModelsFactory;
    [ObservableProperty]
    private TraderViewModel selectedTrader;


    public ObservableCollection<string> AvailableModifiers { get; }
    = new(new[] { "*", "W", "M", "V", "VNK", "K", "S" });
    public ObservableCollection<TraderViewModel> Traders { get; } = new();
    public CurrencyCategory CurrencyCategory
    {
        get => _model.CurrencyCategory;
        set => SetProperty(_model.CurrencyCategory, value, _model, (m, n) => m.CurrencyCategory = n);
    }

    public TraderConfigViewModel(string fileName, TraderConfig model, IValidator<TraderConfig> validator, IDialogFactory dialogFactory,
        TraderViewModelsFactory viewModelsFactory) : base(fileName, model, validator, dialogFactory)
    {
        _viewModelsFactory = viewModelsFactory;
        
        CurrencyCategory = model.CurrencyCategory;
        Traders.AddRange(model.Traders.Select(t => _viewModelsFactory.CreateTraderViewModel(t)));

        Traders.CollectionChanged += TradersCollectionChanged;
    }

    protected override bool Validate()
    {
        WeakReferenceMessenger.Default.Send(new ClearValidationErrorsMessage(this));

        var itemsErrors = Traders.AsParallel()
          .Select(trader => new { trader.Name, Result = trader.ValidateSelf() })
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

    private void TradersCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                foreach (var item in e.NewItems)
                {
                    Model.Traders.Add(((TraderViewModel)item).Model);
                }
                break;
            case NotifyCollectionChangedAction.Remove:
                foreach (var item in e.OldItems)
                {
                    Model.Traders.Remove(((TraderViewModel)item).Model);
                }
                break;
            default:
                break;
        }
    }

    public void Dispose()
    {
        Traders.CollectionChanged -= TradersCollectionChanged;
    }
}
