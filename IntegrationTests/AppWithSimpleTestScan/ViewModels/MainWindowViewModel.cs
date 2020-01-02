using System.Diagnostics;
using CmpCurvesSummation.Core;

namespace AppWithSimpleTestScan.ViewModels
{

    public delegate void FileOpenHandler(object obj, FileLoadedEventArgs e);



    public class MainWindowViewModel
    {
        public MainWindowViewModel()
        {
        }


        public void DataLoaded(object obj, FileLoadedEventArgs args)
        {
            var _cmpScan = args.CmpScan;

            ProcessData(_cmpScan);
        }

        private void ProcessData(ICmpScan cmpScan)
        {

        }

    }
}