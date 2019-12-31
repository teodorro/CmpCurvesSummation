using System.Collections.ObjectModel;

namespace CmpCurvesSummation.Core.Processing
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
        void AutoDetectProcessingsToUse(CmpScan data);
        ObservableCollection<IRawDataProcessing> Operations { get; }
        void Process(ICmpScan cmpScan);
    }


    public class RawDataProcessor : IRawDataProcessor
    {
        public ObservableCollection<IRawDataProcessing> Operations { get; } = new ObservableCollection<IRawDataProcessing>();

        public void Process(ICmpScan cmpScan)
        {
            cmpScan.CopyRawDataToProcessed();
            foreach (var operation in Operations)
                operation.Process(cmpScan);
        }

        public void AutoDetectProcessingsToUse(CmpScan data)
        {

        }
    }
}