using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CmpCurvesSummation.Core;
using ProcessingModule.Annotations;

namespace ProcessingModule.ViewModels
{
    public class SummedScanProcessingViewModel : INotifyPropertyChanged
    {
        private ISummedScanVT _summedScan;

        public ObservableCollection<SumProcessingDataRow> ProcessingRowList { get; } = new ObservableCollection<SumProcessingDataRow>();
        public ISummedScanProcessor Processor { get; }


        public SummedScanProcessingViewModel(ISummedScanProcessor processor)
        {
            EventAggregator.Instance.SummationFinished += OnSummationFinished;
            Processor = processor;
            processor.InitOperationList();
            InitOperationsList();
        }

        private void InitOperationsList()
        {
            ProcessingRowList.Clear();

            foreach (var operation in Processor.OperationsAvailable)
            {
                var processingDataRow = new SumProcessingDataRow(false, operation);
                processingDataRow.ProcessingListChanged += OnProcessingListChanged;
                ProcessingRowList.Add(processingDataRow);
            }
        }
        
        internal void OnProcessingListChanged(object sender, SumProcessingListChangedEventArgs e)
        {
            UpdateProcessingList(e);
            Processor.Process(_summedScan);
            EventAggregator.Instance.Invoke(this, new SumDataProcessedEventArgs(_summedScan));
        }

        private void UpdateProcessingList(SumProcessingListChangedEventArgs e)
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

        private void OnSummationFinished(object sender, SummationFinishedEventArgs e)
        {
            _summedScan = e.SummedScan;
            Processor.RefreshOperations(_summedScan);
            Processor.Process(_summedScan);
            EventAggregator.Instance.Invoke(this, new SumDataProcessedEventArgs(_summedScan));
        }


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