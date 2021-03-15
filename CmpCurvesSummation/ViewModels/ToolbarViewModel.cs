using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using CmpCurvesSummation.Core;
using CmpScanModule.Annotations;
using GprFileService;
using Microsoft.Win32;

namespace CmpCurvesSummation.ViewModels
{
    public class ToolbarViewModel : INotifyPropertyChanged
    {
        private readonly IFileOpener _fileOpener;


        public ToolbarViewModel()
        {
            _fileOpener = DiContainer.Instance.Container.GetInstance<IFileOpener>(); ;

            EventAggregator.Instance.CmpDataProcessed += OnCmpDataProcessed;

            OpenFileCommand = new RelayCommand<object>(_ => OpenFile());
            LaunchSummationCommand = new RelayCommand<object>(_ => LaunchSummation());
        }

        public ICommand OpenFileCommand { get; set; }
        public ICommand LaunchSummationCommand { get; set; }
        
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
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnCmpDataProcessed(object obj, CmpDataProcessedEventArgs args)
        {
            ManualSummationPossible = true;
        }
 
        public void LaunchSummation()
        {
            EventAggregator.Instance.Invoke(this, new SummationStartedEventArgs());
        }


        public event PropertyChangedEventHandler PropertyChanged;
        
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}