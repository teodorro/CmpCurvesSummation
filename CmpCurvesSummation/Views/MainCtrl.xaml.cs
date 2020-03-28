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
using CmpCurvesSummation.ViewModels;

namespace CmpCurvesSummation.Views
{
    /// <summary>
    /// Interaction logic for MainCtrl.xaml
    /// </summary>
    public partial class MainCtrl : UserControl
    {

        public MainCtrl()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ToolbarControl.ViewModel.OpenFile();
        }
    }
}
