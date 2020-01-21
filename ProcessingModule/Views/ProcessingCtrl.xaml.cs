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
using ProcessingModule.Processing;
using ProcessingModule.ViewModels;

namespace ProcessingModule.Views
{
    /// <summary>
    /// Interaction logic for ProcessingCtrl.xaml
    /// </summary>
    public partial class ProcessingCtrl : UserControl
    {
        private ProcessingViewModel _viewModel;
        public ProcessingViewModel ViewModel => _viewModel;


        public ProcessingCtrl()
        {
            InitializeComponent();

            _viewModel = new ProcessingViewModel(DiContainer.Instance.Container.GetInstance<IRawDataProcessor>());
            DataContext = _viewModel;
        }


        private void ProcessingListDataGrid_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            var dataRow = ((DataGridRow) e.Source).Item as ProcessingDataRow;
            if (dataRow == null)
                return;
            if (dataRow.Processing is ClearOffsetAscans)
            {
                dataRow.Enabled = true;
                var сlearAppearanceAscans = dataRow.Processing as ClearOffsetAscans;
                var dialog = new ClearOffsetAscansWindow(сlearAppearanceAscans.NumberOfAscans, ViewModel.OnProcessingListChanged);
                if (dialog.ShowDialog() == true)
                    сlearAppearanceAscans.NumberOfAscans = dialog.NumberOfOffsetAscans;
            }
            else if (dataRow.Processing is StraightenSynchronizationLine)
            {
                dataRow.Enabled = true;
                var straightenSynchronizationLine = dataRow.Processing as StraightenSynchronizationLine;
                var dialog = new StraightenSynchronizationLineWindow(straightenSynchronizationLine.MinAmplitudeToCheck);
                if (dialog.ShowDialog() == true)
                    straightenSynchronizationLine.MinAmplitudeToCheck = dialog.MinAmplitudeToCheck;

            }
        }
    }
}
