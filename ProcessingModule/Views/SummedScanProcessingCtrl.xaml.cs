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
        private SummedScanProcessingViewModel _viewModel;
        public SummedScanProcessingViewModel ViewModel => _viewModel;

        private const int _tempCtrlIndex = 3;

        public SummedScanProcessingCtrl()
        {
            InitializeComponent();

            _viewModel = new SummedScanProcessingViewModel(DiContainer.Instance.Container.GetInstance<ISummedScanProcessor>());
            DataContext = _viewModel;
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
            UiElementsStack.Children.Add(new ChangeMaxVelocityCtrl(_viewModel.OnProcessingListChanged, processing));
        }

        private void ManageRaiseToPower(RaiseToPower processing)
        {
            UiElementsStack.Children.Add(new RaiseToPowerCtrl(_viewModel.OnProcessingListChanged, processing));
        }
    }
}
