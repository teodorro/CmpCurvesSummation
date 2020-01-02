using System;
using System.Collections.ObjectModel;
using CmpCurvesSummation.Core;
using ProcessingModule.Processing;

namespace ProcessingModule.ViewModels
{
    public class ProcessingViewModel
    {
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
                ProcessingRowList.Add(new ProcessingDataRow()
                {
                    Processing = operation,
                    Enabled = enabled
                });
                if (enabled)
                    Processor.OperationsToProcess.Add(operation);
            }
        }

        // TODO: should be placed in config or something
        private bool IsOperationEnabled(IRawDataProcessing operation)
        {
            if (operation is Smoothing || operation is LogarithmProcessing)
                return false;
            return true;
        }

        public void OnFileLoaded(object sender, FileLoadedEventArgs e)
        {
            var cmpScan = e.CmpScan;
            Processor.Process(cmpScan);
        }
    }


    public class ProcessingDataRow
    {
        public string Name => Processing.ToString();
        public bool Enabled { get; set; }
        public IRawDataProcessing Processing { get; set; }
    }
}