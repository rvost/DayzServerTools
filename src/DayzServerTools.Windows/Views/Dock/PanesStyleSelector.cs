using System.Windows.Controls;
using System.Windows;

using DayzServerTools.Application.ViewModels.Base;
using DayzServerTools.Application.ViewModels.Panes;

namespace DayzServerTools.Windows.Views.Dock;

class PanesStyleSelector : StyleSelector
{
    public Style PaneStyle { get; set; }
    public Style DocumentStyle { get; set; }

    public override Style SelectStyle(object item, DependencyObject container)
    {
        if(item is IPane)
        {
            return PaneStyle;
        }
        if (item is IProjectFileTab)
        {
            return DocumentStyle;
        }

        return base.SelectStyle(item, container);
    }
}
