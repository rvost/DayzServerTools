using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using DayzServerTools.Library.Trader;
using System.ComponentModel.DataAnnotations;

namespace DayzServerTools.Application.ViewModels;

public class TraderItemViewModel : ObservableValidator
{
    private readonly TraderItem _model;

    public TraderItem Model => _model;
    [MinLength(1)]
    public string Name
    {
        get => _model.Name;
        set => SetProperty(_model.Name, value, _model, (m, v) => m.Name = v, true);
    }
    [RegularExpression(@"^(\*|W|M|V|VNK|K|S|[1-9]\d*)$", ErrorMessage ="Invalid Quantity Modifier")]
    public string Modifier
    {
        get => _model.Modifier;
        set => SetProperty(_model.Modifier, value, _model, (m, v) => m.Modifier = v, true);
    }
    [Range(-1, double.MaxValue)]
    public double BuyPrice
    {
        get => _model.BuyPrice;
        set => SetProperty(_model.BuyPrice, value, _model, (m, v) => m.BuyPrice = v, true);
    }
    [Range(-1, double.MaxValue)]
    public double SellPrice
    {
        get => _model.SellPrice;
        set => SetProperty(_model.SellPrice, value, _model, (m, v) => m.SellPrice = v, true);
    }

    public IRelayCommand ProhibitBuyingCommand { get; }
    public IRelayCommand ProhibitSellingCommand { get; }
    
    public TraderItemViewModel(TraderItem model)
    {
        _model = model;

        ProhibitBuyingCommand = new RelayCommand(ProhibitBuying);
        ProhibitSellingCommand = new RelayCommand(ProhibitSelling);
    }

    public void ValidateSelf() => ValidateAllProperties();
    protected void ProhibitBuying()
    {
        BuyPrice = -1;
    }
    protected void ProhibitSelling()
    {
        SellPrice = -1;
    }
}
