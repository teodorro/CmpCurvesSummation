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
using ProcessingModule.ViewModels;

namespace ProcessingModule.Views
{
    /// <summary>
    /// Interaction logic for ClearAppearanceAscansCtrl.xaml
    /// </summary>
    public partial class ClearAppearanceAscansCtrl : Window
    {
        private ClearAppearanceAscansViewModel _viewModel;

        public ClearAppearanceAscansCtrl()
        {
            InitializeComponent();

            _viewModel = new ClearAppearanceAscansViewModel();
            DataContext = _viewModel;
        }
    }
}
