using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CmpCurvesSummation.Core;
using ProcessingModule.Annotations;

namespace ProcessingModule.ViewModels
{
    public class SummedScanProcessingViewModel : INotifyPropertyChanged
    {
        private ISummedScanVT _summedScan;

//        public event SummedScanProcessedHandler SummedScanDataProcessed;


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }


    public delegate void SumProcessingListChangedHandler(object obj, SumProcessingListChangedEventArgs e);

    public class SumProcessingListChangedEventArgs : EventArgs
    {
        public ISumScanProcessing Processing { get; set; }
        public bool? Enabled { get; set; }
    }


    public class SumProcessingDataRow : INotifyPropertyChanged
    {
        public SumProcessingDataRow(bool enabled, ISumScanProcessing processing)
        {
            _enabled = enabled;
            _processing = processing;
        }

        public event SumProcessingListChangedHandler ProcessingListChanged;

        public string Name => Processing.ToString();

        private bool _enabled;
        public bool Enabled
        {
            get => _enabled;
            set
            {
                _enabled = value;
                ProcessingListChanged.Invoke(this, new SumProcessingListChangedEventArgs()
                {
                    Processing = _processing,
                    Enabled = _enabled
                });
                OnPropertyChanged(nameof(Enabled));
            }
        }

        private ISumScanProcessing _processing;
        public ISumScanProcessing Processing => _processing;


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}