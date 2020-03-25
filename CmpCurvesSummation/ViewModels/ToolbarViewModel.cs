using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using CmpCurvesSummation.Core;
using CmpScanModule.Annotations;
using GprFileService;
using Microsoft.Win32;

namespace CmpCurvesSummation.ViewModels
{


    public interface IToolbarViewModel
    {
        void OpenFile();
    }



    public class ToolbarViewModel : IToolbarViewModel, INotifyPropertyChanged
    {
        private IFileOpener _fileOpener;


        public ToolbarViewModel(IFileOpener fileOpener)
        {
            _fileOpener = fileOpener;

            EventAggregator.Instance.SummationInProcess += OnSummationInProcess;
            EventAggregator.Instance.SumDataProcessed += OnSumProcessed;
            EventAggregator.Instance.CmpDataProcessed += OnCmpDataProcessed;
        }


        private bool _manualSummation;
        public bool ManualSummationPossible
        {
            get => _manualSummation;
            set
            {
                _manualSummation = value;
                OnPropertyChanged(nameof(ManualSummationPossible));
            }
        }

        private bool _progressBarVisible = false;
        public Visibility ProgressBarVisible
        {
            get => _progressBarVisible ? Visibility.Visible : Visibility.Hidden;
            set
            {
                _progressBarVisible = value == Visibility.Visible;
                OnPropertyChanged(nameof(ProgressBarVisible));
            }
        }

        private int _progressValue;
        public int ProgressValue
        {
            get => _progressValue;
            set
            {
                _progressValue = value;
                OnPropertyChanged(nameof(ProgressValue));
            }
        }


        public void OpenFile()
        {
            try
            {
                var fileDialog = new OpenFileDialog();
                fileDialog.Filter =
                    "Geo Files (*.geo)|*.geo|Gem Files (*.gem)|*.gem|Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
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
                        case "gem":
                            data = _fileOpener.OpenGem(fileDialog.FileName);
                            break;
                        default:
                            data = _fileOpener.OpenKrotTxt(fileDialog.FileName);
                            break;
                    }

                    EventAggregator.Instance.Invoke(this, new FileLoadedEventArgs(data, fileDialog.FileName));

//                    if (geo)
//                        SecondAttemptForGeo(fileDialog);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SecondAttemptForGeo(OpenFileDialog fileDialog)
        {
            ICmpScan data;
            var isOk = MessageBox.Show("Нормально открылось?", "", MessageBoxButton.YesNo);
            if (isOk == MessageBoxResult.No)
            {
                data = _fileOpener.OpenGeo1(fileDialog.FileName);
                EventAggregator.Instance.Invoke(this, new FileLoadedEventArgs(data, fileDialog.FileName));
            }
        }

        public void LaunchSummation()
        {
            ProgressBarVisible = Visibility.Visible;
            EventAggregator.Instance.Invoke(this, new SummationStartedEventArgs());
        }

        private void OnSumProcessed(object obj, SumDataProcessedEventArgs e)
        {
            ProgressBarVisible = Visibility.Hidden;
            ProgressValue = 0;
        }

        private void OnSummationInProcess(object obj, SummationInProcessEventArgs e)
        {
            ProgressValue = e.Percent;
        }

        private void OnCmpDataProcessed(object obj, CmpDataProcessedEventArgs args)
        {
            ManualSummationPossible = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}