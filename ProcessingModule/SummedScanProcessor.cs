using System.Collections.Generic;
using System.Collections.ObjectModel;
using CmpCurvesSummation.Core;
using ProcessingModule.Processing.SummedScan;

namespace ProcessingModule
{
    public interface ISumScanProcessing
    {
        string Name { get; }
        void Process(ISummedScanVT summedScan);
        int OrderIndex { get; }
    }


    public interface ISummedScanProcessor
    {
        void InitOperationList();
        ObservableCollection<ISumScanProcessing> OperationsAvailable { get; }
        List<ISumScanProcessing> OperationsToProcess { get; }
        void Process(ISummedScanVT summedScan);
        void AutoDetectOperationsNeeded(ISummedScanVT summedScan);
        void RefreshOperations(ISummedScanVT summedScan);
    }


    public class SummedScanProcessor : ISummedScanProcessor
    {
        public SummedScanProcessor()
        {
            InitOperationList();
        }


        public ObservableCollection<ISumScanProcessing> OperationsAvailable { get; } = new ObservableCollection<ISumScanProcessing>();
        public List<ISumScanProcessing> OperationsToProcess { get; } = new List<ISumScanProcessing>();


        public void InitOperationList()
        {
            OperationsAvailable.Add(new ChangeMaxVelocity());
            OperationsAvailable.Add(new HideWeakValues());
            OperationsAvailable.Add(new RaiseToPower());
            OperationsAvailable.Add(new Absolutize());
        }

        public void Process(ISummedScanVT summedScan)
        {
            if (summedScan == null)
                return;
            summedScan.CopyRawDataToProcessed();
            foreach (var operation in OperationsToProcess)
            {
                operation.Process(summedScan);
            }
        }

        public void AutoDetectOperationsNeeded(ISummedScanVT summedScan)
        {
            throw new System.NotImplementedException();
        }

        public void RefreshOperations(ISummedScanVT summedScan)
        {
            
        }
    }
}