using CmpCurvesSummation.Core;

namespace ProcessingModule.Processing
{
    public class LogarithmProcessing : IRawDataProcessing
    {
        public string Name { get; } = "Логарифм амплитуды";

        public override string ToString() => Name;


        public void Process(ICmpScan cmpScan)
        {
            throw new System.NotImplementedException();
        }
    }
}