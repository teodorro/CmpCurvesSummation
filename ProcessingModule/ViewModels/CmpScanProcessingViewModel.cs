using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using CmpCurvesSummation.Core;
using ProcessingModule.Annotations;
using ProcessingModule.Processing.CmpScan;

namespace ProcessingModule.ViewModels
{

    public class CmpScanProcessingViewModel : INotifyPropertyChanged
    {
        private ICmpScan _cmpScan;
        
        public ObservableCollection<CmpProcessingDataRow> ProcessingRowList { get; } = new ObservableCollection<CmpProcessingDataRow>();
        public ICmpScanProcessor Processor { get; }


        public CmpScanProcessingViewModel(ICmpScanProcessor processor)
        {
            EventAggregator.Instance.FileLoaded += OnFileLoaded;
            Processor = processor;
            processor.InitOperationList();
            InitOperationsList();
        }


        private void InitOperationsList()
        {
            ProcessingRowList.Clear();

            foreach (var operation in Processor.OperationsAvailable)
            {
                var enabled = IsOperationEnabled(operation);
                var processingDataRow = new CmpProcessingDataRow(enabled, operation);
                processingDataRow.ProcessingListChanged += OnProcessingListChanged;
                ProcessingRowList.Add(processingDataRow);
                if (enabled)
                    Processor.OperationsToProcess.Add(operation);
            }
        }

        // TODO: should be placed in config or something
        private bool IsOperationEnabled(ICmpScanProcessing operation)
        {
            if (operation is ZeroAmplitudeCorrection)
                return true;
            return false;
        }

        private void OnFileLoaded(object sender, FileLoadedEventArgs e)
        {
            _cmpScan = e.CmpScan;
            Processor.RefreshOperations(_cmpScan);
            Processor.Process(_cmpScan);
            EventAggregator.Instance.Invoke(this, new CmpDataProcessedEventArgs(_cmpScan));
        }

        internal void OnProcessingListChanged(object sender, CmpProcessingListChangedEventArgs e)
        {
            UpdateProcessingList(e);
            Processor.Process(_cmpScan);
            EventAggregator.Instance.Invoke(this, new CmpDataProcessedEventArgs(_cmpScan));
        }

        private void UpdateProcessingList(CmpProcessingListChangedEventArgs e)
        {
            foreach (var row in ProcessingRowList)
            {
                if (row.Processing == e.Processing && e.Enabled != null && row.Enabled != (bool)e.Enabled)
                    row.Enabled = (bool)e.Enabled;
            }
            if (e.Enabled == true && !Processor.OperationsToProcess.Contains(e.Processing))
                Processor.OperationsToProcess.Add(e.Processing);
            else if (e.Enabled == false)
                Processor.OperationsToProcess.Remove(e.Processing);

        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }


    public delegate void CmpProcessingListChangedHandler(object obj, CmpProcessingListChangedEventArgs e);

    public class CmpProcessingListChangedEventArgs : EventArgs
    {
        public ICmpScanProcessing Processing { get; set; }
        public bool? Enabled { get; set; }
    }



    public class CmpProcessingDataRow : INotifyPropertyChanged
    {
        public CmpProcessingDataRow(bool enabled, ICmpScanProcessing processing)
        {
            _enabled = enabled;
            _processing = processing;
        }

        public event CmpProcessingListChangedHandler ProcessingListChanged;

        public string Name => Processing.ToString();

        private bool _enabled;
        public bool Enabled
        {
            get => _enabled;
            set
            {
                _enabled = value;
                ProcessingListChanged.Invoke(this, new CmpProcessingListChangedEventArgs()
                {
                    Processing = _processing,
                    Enabled = _enabled
                });
                OnPropertyChanged(nameof(Enabled));
            }
        }

        private ICmpScanProcessing _processing;
        public ICmpScanProcessing Processing => _processing;


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}