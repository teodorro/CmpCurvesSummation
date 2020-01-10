using System.Windows;
using System.Windows.Controls;
using CmpCurvesSummation.Core;
using CmpCurvesSummation.ViewModels;
using GprFileService;

namespace CmpCurvesSummation.Views
{
    /// <summary>
    /// Interaction logic for ToolbarCtrl.xaml
    /// </summary>
    public partial class ToolbarCtrl : UserControl
    {
        private IToolbarViewModel _viewModel;
        public IToolbarViewModel ViewModel => _viewModel;


        public ToolbarCtrl()
        {
            InitializeComponent();
            var fileOpener = DiContainer.Instance.Container.GetInstance<IFileOpener>();
            _viewModel = new ToolbarViewModel(fileOpener);
            DataContext = _viewModel;
        }

//        private void OpenFile(object sender, ExecutedRoutedEventArgs e)
//        {
//            _viewModel.OpenFile();
//
//        }

        private void ButtonOpenFile_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.OpenFile();
        }
    }
}
