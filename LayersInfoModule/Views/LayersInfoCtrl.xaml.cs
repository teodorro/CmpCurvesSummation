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
using System.Windows.Navigation;
using System.Windows.Shapes;
using CmpCurvesSummation.Core;
using LayersInfoModule.ViewModels;

namespace LayersInfoModule.Views
{
    /// <summary>
    /// Interaction logic for LayersInfoCtrl.xaml
    /// </summary>
    public partial class LayersInfoCtrl : UserControl
    {
        private LayersInfoViewModel _viewModel;
        public LayersInfoViewModel ViewModel => _viewModel;

        public LayersInfoCtrl()
        {
            InitializeComponent();

            _viewModel = new LayersInfoViewModel();
            DataContext = _viewModel;
            LayersListDataGrid.PreviewKeyDown += LayersListDataGridOnKeyDown;
        }

        private void LayersListDataGridOnKeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            if (keyEventArgs.Key == Key.Delete)
            {
                var dataGrid = (DataGrid) sender;
                var layer = (dataGrid.SelectedItem as LayerInfo);
                ViewModel.OnDeleteRowClick(sender, new DeleteLayerEventsArgs(layer.Velocity, layer.Time));
            }
        }
    }
}
