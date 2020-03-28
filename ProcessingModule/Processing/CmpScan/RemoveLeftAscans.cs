using CmpCurvesSummation.Core;

namespace ProcessingModule.Processing.CmpScan
{
    public class RemoveLeftAscans : ICmpScanProcessing
    {
        public const int DefaultValue = 4;
        public string Name { get; } = "Удалить измерения слева";
        public override string ToString() => Name;

        public int NumberOfAscans { get; set; } = DefaultValue;

        private int _maximumNumberOfAscans;
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

        public int OrderIndex { get; } = 0;


        public void Process(ICmpScan cmpScan)
        {
            var data = cmpScan.Data;
            data.RemoveRange(0, NumberOfAscans);
        }
    }
}