using CmpCurvesSummation.Core;
using GprFileService;
using Microsoft.Win32;

namespace AppWithSimpleTestScan.ViewModels
{

    public delegate void FileOpenHandler(object obj, FileLoadedEventArgs e);

    public interface IToolbarViewModel
    {
        event FileOpenHandler FileOpened;
        void OpenFile();
    }



    public class ToolbarViewModel : IToolbarViewModel
    {
        public event FileOpenHandler FileOpened;
        private IFileOpener _fileOpener;


        public ToolbarViewModel(IFileOpener fileOpener)
        {
            _fileOpener = fileOpener;
            //CommandManager.InvalidateRequerySuggested();
        }


        public void OpenFile()
        {
            var fileDialog = new OpenFileDialog();
            if (fileDialog.ShowDialog() == true)
            {
                var data = _fileOpener.OpenKrotTxt(fileDialog.FileName);
                FileOpened.Invoke(this, new FileLoadedEventArgs(data));
            }
        }
    }
}