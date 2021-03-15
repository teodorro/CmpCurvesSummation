using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CmpCurvesSummation.Core;
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


        public CmpScanProcessor()
        {
            InitOperationList();
        }


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
            OperationsAvailable.Add(new RemoveLeftAscans());
            OperationsAvailable.Add(new RemoveRightAscans());
            OperationsAvailable.Add(new ZeroAmplitudeCorrection());
            OperationsAvailable.Add(new LogarithmProcessing());
            OperationsAvailable.Add(new AddOffsetAscans());
            OperationsAvailable.Add(new ClearOffsetAscans());
            OperationsAvailable.Add(new StraightenSynchronizationLine());
        }

        public void RefreshOperations(ICmpScan cmpScan)
        {
            var removeLeftAscans = OperationsAvailable.FirstOrDefault(x => x is RemoveLeftAscans) as RemoveLeftAscans;
            if (removeLeftAscans != null)
            {
                removeLeftAscans.NumberOfAscans = RemoveLeftAscans.DefaultValue;
                removeLeftAscans.MaximumNumberOfAscans = cmpScan.RawData.Count - 1;
            }

            var removeRightAscans = OperationsAvailable.FirstOrDefault(x => x is RemoveRightAscans) as RemoveRightAscans;
            if (removeRightAscans != null)
            {
                removeRightAscans.NumberOfAscans = RemoveRightAscans.DefaultValue;
                removeRightAscans.MaximumNumberOfAscans = cmpScan.RawData.Count - 1;
            }

            var addOffsetAscans = OperationsAvailable.FirstOrDefault(x => x is AddOffsetAscans) as AddOffsetAscans;
            addOffsetAscans.NumberOfAscans = AddOffsetAscans.DefaultValue;

            var clearOffsetAscans = OperationsAvailable.FirstOrDefault(x => x is ClearOffsetAscans) as ClearOffsetAscans;
            clearOffsetAscans.NumberOfAscans = ClearOffsetAscans.DefaultValue;

            var straightenSynchronizationLine = OperationsAvailable.FirstOrDefault(x => x is StraightenSynchronizationLine) as StraightenSynchronizationLine;
            straightenSynchronizationLine.MinNegativeAmplitudeToBegin = StraightenSynchronizationLine.DefaultValueNeg;
            straightenSynchronizationLine.MinPositiveAmplitudeToStop = StraightenSynchronizationLine.DefaultValuePos;
        }
    }
}