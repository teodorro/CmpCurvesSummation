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



//        private void ListViewProcessing_PreviewMouseMove(object sender, MouseEventArgs e)
//        {
//            //            if (e.LeftButton == MouseButtonState.Pressed &&
//            //                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
//            //                 Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
//            if (e.LeftButton == MouseButtonState.Pressed)
//            {
//                var a = e.Source;
//                DragDrop.DoDragDrop(ListViewChoose, a, DragDropEffects.Copy);
//            }
//        }
//
//        private void ListViewChoose_Drop(object sender, DragEventArgs e)
//        {
//            // TODO: it looks somehow strange 
////            this._viewModel.ProcessingList.Add(ListViewProcessing.SelectedItem as IRawDataProcessing);
//        }
    }
}
