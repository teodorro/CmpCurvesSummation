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
using ProcessingModule.Processing.SummedScan;
using ProcessingModule.ViewModels;
using ProcessingModule.Views.SummedScan;

namespace ProcessingModule.Views
{
    /// <summary>
    /// Interaction logic for SummedScanProcessingCtrl.xaml
    /// </summary>
    public partial class SummedScanProcessingCtrl : UserControl
    {
        public SummedScanProcessingViewModel ViewModel => DataContext as SummedScanProcessingViewModel;

        private const int _tempCtrlIndex = 3;

        public SummedScanProcessingCtrl()
        {
            InitializeComponent();

            EventAggregator.Instance.FileLoaded += (o, args) => { IsEnabled = false; };
            EventAggregator.Instance.SummationFinished += (o, args) => { IsEnabled = true; };

            ProcessingListDataGrid.SelectedIndex = 0;

        }


        private void RemoveIrrelevantCtrls()
        {
            if (UiElementsStack.Children.Count > _tempCtrlIndex)
                UiElementsStack.Children.RemoveAt(_tempCtrlIndex);
        }

        private void EventSetter_OnHandler(object sender, RoutedEventArgs e)
        {
            RemoveIrrelevantCtrls();

            var dataRow = ((DataGridRow)e.Source).Item as SumProcessingDataRow;
            if (dataRow == null)
                return;

            if (dataRow.Processing is ChangeMaxVelocity changeMaxVelocity)
                ManageChangeMaxVelocity(changeMaxVelocity);
            else if (dataRow.Processing is RaiseToPower raiseToPower)
                ManageRaiseToPower(raiseToPower);
        }
        
        private void ManageChangeMaxVelocity(ChangeMaxVelocity processing)
        {
            UiElementsStack.Children.Add(new ChangeMaxVelocityCtrl(ViewModel.OnProcessingListChanged, processing));
        }

        private void ManageRaiseToPower(RaiseToPower processing)
        {
            UiElementsStack.Children.Add(new RaiseToPowerCtrl(ViewModel.OnProcessingListChanged, processing));
        }
    }
}
