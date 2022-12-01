using DayzServerTools.Application.ViewModels.ItemTypes;
using DayzServerTools.Library.Xml;

namespace DayzServerTools.Application.Stores;

public class ItemTypesClassnameImportStore : IClassnameImportStore
{
    private readonly ItemTypesViewModel _viewModel;

    public ItemTypesClassnameImportStore(ItemTypesViewModel viewModel)
    {
        _viewModel = viewModel;
    }

    public void Accept(IEnumerable<string> classnames)
    {
        var itemTypes = classnames.Select(name => new ItemType() { Name = name });
        _viewModel.CopyItemTypes(itemTypes);
    }
}
