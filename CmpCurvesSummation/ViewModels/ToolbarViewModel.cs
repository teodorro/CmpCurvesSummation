﻿using CmpCurvesSummation.Core;
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