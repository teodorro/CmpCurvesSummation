using System.Collections.ObjectModel;

namespace CmpCurvesSummation.Core.Processing
{
    public interface IRawDataProcessor
    {
        void AutoDetectProcessingsToUse(CmpScan data);
        ObservableCollection<IRawDataProcessing> Operations { get; }
    }

    public interface IRawDataProcessing
    {
        void Process(ICmpScan data);
    }


    public class RawDataProcessor : IRawDataProcessor
    {
        public ObservableCollection<IRawDataProcessing> Operations { get; } = new ObservableCollection<IRawDataProcessing>();

        public void AutoDetectProcessingsToUse(CmpScan data)
        {

        }
    }
}