using CmpCurvesSummation.Core;

namespace ProcessingModule.Processing
{
    public class AddOffsetAscans : IRawDataProcessing
    {
        public string Name { get; } = "Добавить измерения отступа";
        public override string ToString() => Name;

        public int NumberOfAscans { get; set; } = 5;


        public void Process(ICmpScan cmpScan)
        {
            var data = cmpScan.Data;
            for (int i = 0; i < NumberOfAscans; i++)
                data.Insert(0, new double[cmpScan.AscanLengthDimensionless]);
        }
    }
}