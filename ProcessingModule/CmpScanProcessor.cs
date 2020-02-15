using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CmpCurvesSummation.Core;
using ProcessingModule.Processing;
using ProcessingModule.Processing.CmpScan;

namespace ProcessingModule
{
    /// <summary>
    /// Make some processing on CMP Scan
    /// </summary>
    public interface ICmpScanProcessing
    {
        string Name { get; }
        int OrderIndex { get; }
        void Process(ICmpScan data);
    }

    /// <summary>
    /// Through opening of a file one get a raw cmpScan
    /// In all the cases some processing <see cref="ICmpScanProcessing"/> is needed
    /// Processor contains a list of such operations and they may be launched in a specific order
    /// </summary>
    public interface ICmpScanProcessor
    {
        void InitOperationList();
        ObservableCollection<ICmpScanProcessing> OperationsAvailable { get; }
        List<ICmpScanProcessing> OperationsToProcess { get; }
        void Process(ICmpScan cmpScan);
        void AutoDetectOperationsNeeded(ICmpScan data);
        void RefreshOperations(ICmpScan cmpScan);
    }


    public class CmpScanProcessor : ICmpScanProcessor
    {
        public ObservableCollection<ICmpScanProcessing> OperationsAvailable { get; } =
            new ObservableCollection<ICmpScanProcessing>();
        public List<ICmpScanProcessing> OperationsToProcess { get; } =
            new List<ICmpScanProcessing>();

        public void Process(ICmpScan cmpScan)
        {
            if (cmpScan == null)
                return;
            cmpScan.CopyRawDataToProcessed();
            foreach (var operation in OperationsToProcess)
            {
                operation.Process(cmpScan);
            }
        }

        public void AutoDetectOperationsNeeded(ICmpScan data)
        {
            throw new System.NotImplementedException();
        }

        public void InitOperationList()
        {
            OperationsAvailable.Add(new RemoveLeftAscans(0));
            OperationsAvailable.Add(new RemoveRightAscans(1));
            OperationsAvailable.Add(new ZeroAmplitudeCorrection(2));
            OperationsAvailable.Add(new LogarithmProcessing(3));
            OperationsAvailable.Add(new AddOffsetAscans(4));
            OperationsAvailable.Add(new ClearOffsetAscans(5));
//            OperationsAvailable.Add(new StraightenSynchronizationLine());
            OperationsAvailable.Add(new StraightenSynchronizationLine2(6));
        }

        public void RefreshOperations(ICmpScan cmpScan)
        {
            var removeLeftAscans = OperationsAvailable.FirstOrDefault(x => x is RemoveLeftAscans) as RemoveLeftAscans;
            if (removeLeftAscans != null)
                removeLeftAscans.MaximumNumberOfAscans = cmpScan.RawData.Count;
            var removeRightAscans = OperationsAvailable.FirstOrDefault(x => x is RemoveRightAscans) as RemoveRightAscans;
            if (removeRightAscans != null)
                removeRightAscans.MaximumNumberOfAscans = cmpScan.RawData.Count;
        }
    }
}