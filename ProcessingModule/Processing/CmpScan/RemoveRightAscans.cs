using CmpCurvesSummation.Core;

namespace ProcessingModule.Processing.CmpScan
{
    public class RemoveRightAscans : ICmpScanProcessing
    {
        public string Name { get; } = "Удалить измерения справа";
        public override string ToString() => Name;

        public int NumberOfAscans { get; set; } = 0;

        private int _maximumNumberOfAscans = 0;
        public int MaximumNumberOfAscans
        {
            get => _maximumNumberOfAscans;
            set
            {
                _maximumNumberOfAscans = value;
                if (NumberOfAscans > _maximumNumberOfAscans)
                    NumberOfAscans = _maximumNumberOfAscans;
            }
        }

        public int OrderIndex { get; }


        public RemoveRightAscans(int orderIndex)
        {
            OrderIndex = orderIndex;
        }


        public void Process(ICmpScan cmpScan)
        {
            var data = cmpScan.Data;
            data.RemoveRange(cmpScan.Data.Count - NumberOfAscans, NumberOfAscans);
        }
    }
}