using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DayzServerTools.Application.Models
{
    public interface ISpawnableTypesExportDialog
    {
        IEnumerable<string> Classnames { get; set; }
        bool? ShowDialog();
    }
}
