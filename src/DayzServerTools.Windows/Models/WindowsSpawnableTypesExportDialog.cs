using System.Collections.Generic;

using DayzServerTools.Application.Models;
using DayzServerTools.Application.ViewModels.Dialogs;
using DayzServerTools.Windows.Views.Dialogs;

namespace DayzServerTools.Windows.Models;

internal class WindowsSpawnableTypesExportDialog : ISpawnableTypesExportDialog
{
    public IEnumerable<string> Classnames { get; set; }

    public bool? ShowDialog()
    {
        var vm = new SpawnableTypesExportViewModel(Classnames);
        var window = new SpawnableTypesExportView(vm);

        return window.ShowDialog();
    }
}
