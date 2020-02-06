using CmpCurvesSummation.Core;

namespace ProcessingModule.Processing
{
    public class RemoveLeftAscans : IRawDataProcessing
    {
        public string Name { get; } = "Удалить измерения слева";
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


        public void Process(ICmpScan cmpScan)
        {
            var data = cmpScan.Data;
            data.RemoveRange(0, NumberOfAscans);
        }
    }
}