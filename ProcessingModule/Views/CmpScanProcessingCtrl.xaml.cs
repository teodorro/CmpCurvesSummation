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
using ProcessingModule.Processing.CmpScan;
using ProcessingModule.ViewModels;

namespace ProcessingModule.Views
{
    /// <summary>
    /// Interaction logic for CmpScanProcessingCtrl.xaml
    /// </summary>
    public partial class CmpScanProcessingCtrl : UserControl
    {
        private const int _tempCtrlIndex = 2;


        public CmpScanProcessingCtrl()
        {
            InitializeComponent();

            EventAggregator.Instance.FileLoaded += (o, args) => { this.IsEnabled = true; };
        }


//        private void RemoveIrrelevantCtrls()
//        {
//            if (UiElementsStack.Children.Count > _tempCtrlIndex)
//                UiElementsStack.Children.RemoveAt(_tempCtrlIndex);
//        }

//        private void EventSetter_OnHandler(object sender, RoutedEventArgs e)
//        {
//            RemoveIrrelevantCtrls();
//
//            var dataRow = ((DataGridRow)e.Source).Item as CmpProcessingDataRow;
//            if (dataRow == null)
//                return;
//
//            if (dataRow.Processing is ClearOffsetAscans clearOffsetAscans)
//                ManageClearOffsetAscans(clearOffsetAscans);
//            else if (dataRow.Processing is StraightenSynchronizationLine straightenSynchronizationLine)
//                ManageStraightenSynchronizationLine(straightenSynchronizationLine);
//            else if (dataRow.Processing is AddOffsetAscans addOffsetAscans)
//                ManageAddOffsetAscans(addOffsetAscans);
//            else if (dataRow.Processing is RemoveLeftAscans removeLeftAscans)
//                ManageRemoveLeftAscans(removeLeftAscans);
//            else if (dataRow.Processing is RemoveRightAscans removeRightAscans)
//                ManageRemoveRightAscans(removeRightAscans);
//            else if (dataRow.Processing is StraightenSynchronizationLine straightenSynchronizationLine2)
//                ManageStraightenSynchronizationLine2(straightenSynchronizationLine2);
//        }

//        private void ManageRemoveRightAscans(RemoveRightAscans processing)
//        {
//            UiElementsStack.Children.Add(new CmpScan.RemoveRightAscansCtrl(ViewModel.OnProcessingListChanged, processing));
//        }
//
//        private void ManageRemoveLeftAscans(RemoveLeftAscans processing)
//        {
//            var removeLeftAscansCtrl = new CmpScan.RemoveLeftAscansCtrl(ViewModel.OnProcessingListChanged, processing);
//            UiElementsStack.Children.Add(removeLeftAscansCtrl);
//        }
//
//        private void ManageAddOffsetAscans(AddOffsetAscans processing)
//        {
//            UiElementsStack.Children.Add(new CmpScan.AddOffsetAscansCtrl(ViewModel.OnProcessingListChanged, processing));
//        }
//
//        private void ManageStraightenSynchronizationLine(StraightenSynchronizationLine processing)
//        {
//            UiElementsStack.Children.Add(new CmpScan.StraightenSynchronizationLineCtrl(ViewModel.OnProcessingListChanged, processing));
//        }
//
//        private void ManageClearOffsetAscans(ClearOffsetAscans processing)
//        {
//            UiElementsStack.Children.Add(new CmpScan.ClearOffsetAscansCtrl(ViewModel.OnProcessingListChanged, processing));
//        }
//
//        private void ManageStraightenSynchronizationLine2(StraightenSynchronizationLine processing)
//        {
//            UiElementsStack.Children.Add(new CmpScan.StraightenSynchronizationLineCtrl(ViewModel.OnProcessingListChanged, processing));
//        }
    }
}
