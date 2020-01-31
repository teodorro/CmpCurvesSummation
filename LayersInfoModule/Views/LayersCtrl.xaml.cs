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
    /// Interaction logic for LayersCtrl.xaml
    /// </summary>
    public partial class LayersCtrl : UserControl
    {
        private LayersViewModel _viewModel;
        public LayersViewModel ViewModel => _viewModel;


        public LayersCtrl()
        {
            InitializeComponent();

            _viewModel = new LayersViewModel();
            DataContext = _viewModel;
            LayersListDataGrid.PreviewKeyDown += LayersListDataGridOnKeyDown;
        }


        private void LayersListDataGridOnKeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            if (keyEventArgs.Key == Key.Delete)
            {
                var dataGrid = (DataGrid) sender;
                var layer = (dataGrid.SelectedItem as LayerInfo);
                ViewModel.OnDeleteRowClick(sender, new DeleteLayerEventArgs(layer.AvgVelocity, layer.Time));
            }
        }

        private void LayersListDataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            e.Column.Header = ((System.ComponentModel.MemberDescriptor) e.PropertyDescriptor).DisplayName;
        }

        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem)sender;
            var contextMenu = (ContextMenu)menuItem.Parent;
            var item = (DataGridRow)contextMenu.PlacementTarget;
            var layer = item.DataContext as LayerInfo;

            ViewModel.OnDeleteRowClick(sender, new DeleteLayerEventArgs(layer.AvgVelocity, layer.Time));
        }
    }
}
