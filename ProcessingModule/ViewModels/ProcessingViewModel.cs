using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using CmpCurvesSummation.Core;
using ProcessingModule.Annotations;
using ProcessingModule.Processing;

namespace ProcessingModule.ViewModels
{

    public class ProcessingViewModel : INotifyPropertyChanged
    {
        private ICmpScan _cmpScan;

        public event RawCmpProcessedHandler RawCmpDataProcessed;

        public ObservableCollection<ProcessingDataRow> ProcessingRowList { get; } = new ObservableCollection<ProcessingDataRow>();
        public IRawDataProcessor Processor { get; }


        public ProcessingViewModel(IRawDataProcessor processor)
        {
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
                var processingDataRow = new ProcessingDataRow(enabled, operation);
                processingDataRow.ProcessingListChanged += OnProcessingListChanged;
                ProcessingRowList.Add(processingDataRow);
                if (enabled)
                    Processor.OperationsToProcess.Add(operation);
            }
        }

        // TODO: should be placed in config or something
        private bool IsOperationEnabled(IRawDataProcessing operation)
        {
            if (operation is Smoothing 
                || operation is LogarithmProcessing 
                || operation is ZeroAmplitudeCorrection 
                || operation is ClearOffsetAscans)
                return false;
            return false;
        }

        public void OnFileLoaded(object sender, FileLoadedEventArgs e)
        {
            _cmpScan = e.CmpScan;
            Processor.Process(_cmpScan);
            RawCmpDataProcessed?.Invoke(this, new RawCmpProcessedEventArgs(_cmpScan));
        }

        internal void OnProcessingListChanged(object sender, ProcessingListChangedEventArgs e)
        {
            if (e.Enabled == true && !Processor.OperationsToProcess.Contains(e.Processing))
                Processor.OperationsToProcess.Add(e.Processing);
            else if (e.Enabled == false) 
                Processor.OperationsToProcess.Remove(e.Processing);
            Processor.Process(_cmpScan);
            RawCmpDataProcessed?.Invoke(this, new RawCmpProcessedEventArgs(_cmpScan));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }


    public delegate void ProcessingListChangedHandler(object obj, ProcessingListChangedEventArgs e);

    public class ProcessingListChangedEventArgs : EventArgs
    {
        public IRawDataProcessing Processing { get; set; }
        public bool? Enabled { get; set; }
    }



    public class ProcessingDataRow : INotifyPropertyChanged
    {
        public ProcessingDataRow(bool enabled, IRawDataProcessing processing)
        {
            _enabled = enabled;
            _processing = processing;
        }

        public event ProcessingListChangedHandler ProcessingListChanged;

        public string Name => Processing.ToString();

        private bool _enabled;
        public bool Enabled
        {
            get => _enabled;
            set
            {
                _enabled = value;
                ProcessingListChanged.Invoke(this, new ProcessingListChangedEventArgs()
                {
                    Processing = _processing,
                    Enabled = _enabled
                });
                OnPropertyChanged(nameof(Enabled));
            }
        }

        private IRawDataProcessing _processing;
        public IRawDataProcessing Processing => _processing;


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}