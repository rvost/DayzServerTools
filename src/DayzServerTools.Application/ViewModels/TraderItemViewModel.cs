using CommunityToolkit.Mvvm.ComponentModel;
using DayzServerTools.Library.Trader;

namespace DayzServerTools.Application.ViewModels;

public class TraderItemViewModel : ObservableObject
{
    private readonly TraderItem _model;

    public TraderItem Model => _model;

    public string Name
    {
        get => _model.Name;
        set => SetProperty(_model.Name, value, _model, (m, v) => m.Name = v);
    }
    public string Modifier
    {
        get => _model.Modifier;
        set => SetProperty(_model.Modifier, value, _model, (m, v) => m.Modifier = v);
    }
    public double BuyPrice
    {
        get => _model.BuyPrice;
        set => SetProperty(_model.BuyPrice, value, _model, (m, v) => m.BuyPrice = v);
    }
    public double SellPrice
    {
        get => _model.SellPrice;
        set => SetProperty(_model.SellPrice, value, _model, (m, v) => m.SellPrice = v);
    }

    public TraderItemViewModel(TraderItem model)
    {
        _model = model;
    }
}
