using System.Windows;
using CmpCurvesSummation.Core;
using GprFileService;
using Microsoft.Win32;

namespace CmpCurvesSummation.ViewModels
{


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
        }


        public void OpenFile()
        {
            var fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Geo Files (*.geo)|*.geo|Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            if (fileDialog.ShowDialog() == true)
            {
                ICmpScan data = null;
                bool geo = false;
                var extension = fileDialog.FileName.Substring(fileDialog.FileName.LastIndexOf(".") + 1);
                switch (extension)
                {
                    case "txt":
                        data = _fileOpener.OpenKrotTxt(fileDialog.FileName);
                        break;
                    case "geo":
                        geo = true;
                        data = _fileOpener.OpenGeo1(fileDialog.FileName);
                        break;
                    default:
                        data = _fileOpener.OpenKrotTxt(fileDialog.FileName);
                        break;
                }
                FileOpened?.Invoke(this, new FileLoadedEventArgs(data, fileDialog.FileName));

                if (geo)
                    SecondAttemptForGeo(fileDialog);
            }
        }

        private void SecondAttemptForGeo(OpenFileDialog fileDialog)
        {
            ICmpScan data;
            var isOk = MessageBox.Show("Нормально открылось?", "Нормально открылось?", MessageBoxButton.YesNo);
            if (isOk == MessageBoxResult.No)
            {
                data = _fileOpener.OpenGeo2(fileDialog.FileName);
                FileOpened?.Invoke(this, new FileLoadedEventArgs(data, fileDialog.FileName));
            }
        }
    }
}