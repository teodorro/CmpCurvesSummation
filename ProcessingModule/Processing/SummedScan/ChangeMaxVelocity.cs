using System;
using CmpCurvesSummation.Core;

namespace ProcessingModule.Processing.SummedScan
{
    public class ChangeMaxVelocity : ISumScanProcessing
    {
        public string Name { get; } = "Изменить макс скорость";
        public override string ToString() => Name;

        private double _maxVelocity = CmpMath.SpeedOfLight / 2;
        public double MaxVelocity
        {
            get => _maxVelocity;
            set
            {
                if (value >= CmpMath.Instance.WaterVelocity)
                    throw new ArgumentOutOfRangeException("ChangeMaxVelocity.MaxVelocity");
                _maxVelocity = value;
            }
        }

        public void Process(ISummedScanVT data)
        {
            throw new System.NotImplementedException();
        }
    }
}