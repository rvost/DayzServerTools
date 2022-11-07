using CommunityToolkit.Mvvm.Input;

using DayzServerTools.Application.ViewModels.Base;
using DayzServerTools.Library.Trader;
using DayzServerTools.Library.Trader.Validators;

namespace DayzServerTools.Application.ViewModels.Trader;

public class TraderItemViewModel : ObservableFluentValidator<TraderItem,TraderItemValidator>
{
    public string Name
    {
        get => _model.Name;
        set => SetProperty(_model.Name, value, _model, (m, v) => m.Name = v, true);
    }
    public string Modifier
    {
        get => _model.Modifier;
        set => SetProperty(_model.Modifier, value, _model, (m, v) => m.Modifier = v, true);
    }
    public double BuyPrice
    {
        get => _model.BuyPrice;
        set => SetProperty(_model.BuyPrice, value, _model, (m, v) => m.BuyPrice = v, true);
    }
    public double SellPrice
    {
        get => _model.SellPrice;
        set => SetProperty(_model.SellPrice, value, _model, (m, v) => m.SellPrice = v, true);
    }

    public IRelayCommand ProhibitBuyingCommand { get; }
    public IRelayCommand ProhibitSellingCommand { get; }
    
    public TraderItemViewModel(TraderItem model):base(model, new())
    {
        ProhibitBuyingCommand = new RelayCommand(ProhibitBuying);
        ProhibitSellingCommand = new RelayCommand(ProhibitSelling);
    }

    protected void ProhibitBuying()
    {
        BuyPrice = -1;
    }
    protected void ProhibitSelling()
    {
        SellPrice = -1;
    }
}
