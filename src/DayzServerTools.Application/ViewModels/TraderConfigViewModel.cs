using System.Collections.ObjectModel;
using System.Collections.Specialized;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using DayzServerTools.Application.ViewModels.Base;
using DayzServerTools.Application.Models;
using DayzServerTools.Application.Services;
using DayzServerTools.Application.Extensions;
using DayzServerTools.Library.Trader;

namespace DayzServerTools.Application.ViewModels
{
    public partial class TraderConfigViewModel : ProjectFileViewModel<TraderConfig>, IDisposable
    {
        public ObservableCollection<TraderViewModel> Traders { get; } = new();
        public CurrencyCategory CurrencyCategory
        {
            get => model.CurrencyCategory;
            set => SetProperty(model.CurrencyCategory, value, model, (m, n) => m.CurrencyCategory = n);
        }

        public TraderConfigViewModel(IDialogFactory dialogFactory) : base(dialogFactory)
        {
            Model = new();
            FileName = "TraderConfig.txt";

            Traders.CollectionChanged += TradersCollectionChanged;
        }

        protected override void OnLoad(Stream input, string filename)
        {
            var newModel = TraderConfig.ReadFromStream(input);
            CurrencyCategory = newModel.CurrencyCategory;
            Traders.AddRange(newModel.Traders.Select(t => new TraderViewModel(t)));
        }
        protected override IFileDialog CreateOpenFileDialog()
        {
            var dialog = _dialogFactory.CreateOpenFileDialog();
            dialog.FileName = "TraderConfig*";
            dialog.Filter = "Text|*.txt";
            return dialog;
        }
        protected override bool CanSave() => true;

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
}
