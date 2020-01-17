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
            if (dataRow.Processing is ClearAppearanceAscans)
            {
                var сlearAppearanceAscans = dataRow.Processing as ClearAppearanceAscans;
                var dialog = new ClearAppearanceAscansWindow(сlearAppearanceAscans.NumberOfAscans);
                if (dialog.ShowDialog() == true)
                    сlearAppearanceAscans.NumberOfAscans = dialog.NumberOfAppearanceAscans;
            }
            else if (dataRow.Processing is StraightenSynchronizationLine)
            {
                var straightenSynchronizationLine = dataRow.Processing as StraightenSynchronizationLine;
                var dialog = new StraightenSynchronizationLineWindow(straightenSynchronizationLine.MinAmplitudeToCheck);
                if (dialog.ShowDialog() == true)
                    straightenSynchronizationLine.MinAmplitudeToCheck = dialog.MinAmplitudeToCheck;

            }
        }

        private void ProcessingListDataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            e.Column.Header = ((System.ComponentModel.MemberDescriptor)e.PropertyDescriptor).DisplayName;
            if (((System.ComponentModel.MemberDescriptor) e.PropertyDescriptor).DisplayName ==
                ((System.ComponentModel.MemberDescriptor) e.PropertyDescriptor).Name)
            {
                e.Column.Visibility = Visibility.Hidden;
                e.Column.Width = 0;
            }
            else
                e.Column.Width = DataGridLength.Auto;
        }

        /// <summary>
        /// TODO: what a perversion? :((
        /// </summary>
        private void ProcessingListDataGrid_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 1)
            {
                if (e.OriginalSource as Decorator != null
                    && ((Decorator)e.OriginalSource).Child as ContentPresenter != null
                    && ((ContentPresenter)((Decorator)e.OriginalSource).Child).Content as CheckBox != null)
                {
                    CheckBoxChange(e);
                }
                else if (e.OriginalSource as CheckBox != null)
                {
                    CheckBoxChange(e);
                }
                else if (e.OriginalSource as Rectangle != null
                         && ((FrameworkElement)e.OriginalSource).TemplatedParent as CheckBox != null)
                {
                    CheckBoxChange(e);
                }
            }
        }

        private void CheckBoxChange(MouseButtonEventArgs e)
        {
            var row = (ProcessingDataRow) ((System.Windows.Controls.Primitives.DataGridCellsPresenter) e.Source).Item;
            row.Enabled = !row.Enabled;
        }
    }
}
