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


        private void RemoveIrrelevantCtrls()
        {
            if (UiElementsStack.Children.Count > 2)
                UiElementsStack.Children.RemoveAt(2);
        }

        private void EventSetter_OnHandler(object sender, RoutedEventArgs e)
        {
            RemoveIrrelevantCtrls();

            var dataRow = ((DataGridRow)e.Source).Item as ProcessingDataRow;
            if (dataRow == null)
                return;

            if (dataRow.Processing is ClearOffsetAscans clearOffsetAscans)
                ManageClearOffsetAscans(clearOffsetAscans);
            else if (dataRow.Processing is StraightenSynchronizationLine straightenSynchronizationLine)
                ManageStraightenSynchronizationLine(straightenSynchronizationLine);
            else if (dataRow.Processing is AddOffsetAscans addOffsetAscans)
                ManageAddOffsetAscans(addOffsetAscans);
            else if (dataRow.Processing is StraightenSynchronizationLine2 straightenSynchronizationLine2)
                ManageStraightenSynchronizationLine2(straightenSynchronizationLine2);
            else
            {
                
            }
        }

        private void ManageAddOffsetAscans(AddOffsetAscans processing)
        {
            UiElementsStack.Children.Add(new AddOffsetAscansCtrl(ViewModel.OnProcessingListChanged, processing));
        }

        private void ManageStraightenSynchronizationLine(StraightenSynchronizationLine processing)
        {
            UiElementsStack.Children.Add(new StraightenSynchronizationLineCtrl(ViewModel.OnProcessingListChanged, processing));
        }

        private void ManageClearOffsetAscans(ClearOffsetAscans processing)
        {
            UiElementsStack.Children.Add(new ClearOffsetAscansCtrl(ViewModel.OnProcessingListChanged, processing));
        }

        private void ManageStraightenSynchronizationLine2(StraightenSynchronizationLine2 processing)
        {
            UiElementsStack.Children.Add(new StraightenSynchronizationLine2Ctrl(ViewModel.OnProcessingListChanged, processing));
        }
    }
}
