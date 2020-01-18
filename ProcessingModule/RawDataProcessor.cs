using System.Collections.ObjectModel;
using CmpCurvesSummation.Core;
using ProcessingModule.Processing;

namespace ProcessingModule
{
    /// <summary>
    /// Make some processing on CMP Scan
    /// </summary>
    public interface IRawDataProcessing
    {
        string Name { get; }
        void Process(ICmpScan data);
    }

    /// <summary>
    /// Through opening of a file one get a raw cmpScan
    /// In all the cases some processing <see cref="IRawDataProcessing"/> is needed
    /// Processor contains a list of such operations and they may be launched in a specific order
    /// </summary>
    public interface IRawDataProcessor
    {
        void InitOperationList();
        ObservableCollection<IRawDataProcessing> OperationsAvailable { get; }
        ObservableCollection<IRawDataProcessing> OperationsToProcess { get; }
        void Process(ICmpScan cmpScan);
        void AutoDetectOperationsNeeded(CmpScan data);
    }


    public class RawDataProcessor : IRawDataProcessor
    {
        public ObservableCollection<IRawDataProcessing> OperationsAvailable { get; } =
            new ObservableCollection<IRawDataProcessing>();
        public ObservableCollection<IRawDataProcessing> OperationsToProcess { get; } =
            new ObservableCollection<IRawDataProcessing>();

        public void Process(ICmpScan cmpScan)
        {
            if (cmpScan == null)
                return;
            cmpScan.CopyRawDataToProcessed();
            foreach (var operation in OperationsToProcess)
                operation.Process(cmpScan);
        }

        public void AutoDetectOperationsNeeded(CmpScan data)
        {
            throw new System.NotImplementedException();
        }

        public void InitOperationList()
        {
            OperationsAvailable.Add(new ZeroAmplitudeCorrection());
            OperationsAvailable.Add(new LogarithmProcessing());
            OperationsAvailable.Add(new StraightenSynchronizationLine());
            //OperationsAvailable.Add(new Smoothing());
            OperationsAvailable.Add(new ClearAppearanceAscans());
        }
    }
}