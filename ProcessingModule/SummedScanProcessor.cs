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
        ObservableCollection<ISumScanProcessing> OperationsToProcess { get; }
        void Process(ISummedScanVT summedScan);
        void AutoDetectOperationsNeeded(ISummedScanVT summedScan);
        void RefreshOperations(ISummedScanVT summedScan);
    }


    public class SummedScanProcessor : ISummedScanProcessor
    {
        public void InitOperationList()
        {
            OperationsAvailable.Add(new ChangeMaxVelocity(0));
            OperationsAvailable.Add(new HideWeakValues(1));
            OperationsAvailable.Add(new RaiseToPower(2));
            OperationsAvailable.Add(new Absolutize(3));
        }

        public ObservableCollection<ISumScanProcessing> OperationsAvailable { get; } = new ObservableCollection<ISumScanProcessing>();
        public ObservableCollection<ISumScanProcessing> OperationsToProcess { get; } = new ObservableCollection<ISumScanProcessing>();

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