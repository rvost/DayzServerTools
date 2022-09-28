using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using DayzServerTools.Application.ViewModels;

namespace DayzServerTools.Windows.Views
{
    /// <summary>
    /// Interaction logic for ExportDialogWindow.xaml
    /// </summary>
    public partial class ExportDialogWindow : Window
    {
        private readonly ExportViewModel _model;

        public ExportDialogWindow(ExportViewModel model)
        {
            InitializeComponent();
            _model = model;
            DataContext = model;
            model.CloseRequested += OnCloseRequested;
        }

        private void OnCloseRequested(object sender, EventArgs e)
        {
            DialogResult = true;
            _model.CloseRequested -= OnCloseRequested;
            Close();
        }
    }
}
