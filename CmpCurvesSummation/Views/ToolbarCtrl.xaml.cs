using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
        public ToolbarCtrl()
        {
            InitializeComponent();
            Focusable = true;
            Loaded += (s, e) => Keyboard.Focus(this);
        }

        private void ToolBar_Loaded(object sender, RoutedEventArgs e)
        {
            (this.DataContext as ToolbarViewModel).OpenFileCommand.Execute(null);
        }
    }
}
