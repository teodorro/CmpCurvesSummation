using CmpCurvesSummation.Core;

namespace ProcessingModule.Processing.CmpScan
{
    public class AddOffsetAscans : ICmpScanProcessing
    {
        public string Name { get; } = "Добавить измерения отступа";
        public override string ToString() => Name;

        public int NumberOfAscans { get; set; } = 5;

        public int OrderIndex { get; }


        public AddOffsetAscans(int orderIndex)
        {
            OrderIndex = orderIndex;
        }


        public void Process(ICmpScan cmpScan)
        {
            var data = cmpScan.Data;
            for (int i = 0; i < NumberOfAscans; i++)
                data.Insert(0, new double[cmpScan.AscanLengthDimensionless]);
        }
    }
}