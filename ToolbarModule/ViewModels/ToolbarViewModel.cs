using System.Windows.Input;
using CmpCurvesSummation.Core;
using CmpFileService;
using Microsoft.Win32;

namespace ToolbarModule.ViewModels
{

    public delegate void FileOpenHandler(object obj, FileLoadedEventArgs e);

    public interface IToolbarViewModel
    {
        event FileOpenHandler OnFileOpened;
        void OpenFile();
    }



    public class ToolbarViewModel : IToolbarViewModel
    {
        public event FileOpenHandler OnFileOpened;
        private IFileOpener _fileOpener;


        public ToolbarViewModel(IFileOpener fileOpener)
        {
            _fileOpener = fileOpener;
            CommandManager.InvalidateRequerySuggested();
        }


        public void OpenFile()
        {
            var fileDialog = new OpenFileDialog();
            if (fileDialog.ShowDialog() == true)
            {
                var data = _fileOpener.OpenKrotTxt(fileDialog.FileName);
                OnFileOpened.Invoke(this, new FileLoadedEventArgs(data));
            }
        }
    }
}