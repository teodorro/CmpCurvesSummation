using System.Collections.Generic;
using System.Linq;

namespace CmpCurvesSummation.Core.Processing
{
    /// <summary>
    /// Krot software uses only not negative numbers for values of amplitude. Which conflicts with physical sense
    /// So for processing operations its better to correct the data
    /// Unfortunately it's format is not stable and there are different ways to describe amplitudes
    /// For some reason krot seems separate all the data to pieces of 100 ascans length. And empty pieces are with amplitude = 0
    /// 1 case: the data amplitude range 1-127, already logarithmic
    /// 2 case: something about ~0-50**, ?
    /// 3 case: not logarithmic amplitudes
    /// </summary>
    public class ZeroAmplitudeCorrection : IRawDataProcessing
    {
        private List<double[]> _data;
        public string Name { get; } = "Корректировка нулевой амплитуды";

        public void Process(ICmpScan cmpScan)
        {
            _data = cmpScan.Data;
            var max = _data.Select(x => x.Where(y => y != 0).Max()).Max();
            var min = _data.Select(x => x.Where(y => y != 0).Min()).Min();

            if (max < 128)
            {
                Convert128();
            }
            else if (max < 6000)
            {
                Convert6000();
            }
            else
            {
                ConvertLinearAmplitudes();
            }
        }

        private void Convert128()
        {
            var zeroAmp = 64;
            foreach (var ascan in _data)
                for (int i = 0; i < ascan.Length; i++)
                    ascan[i] = ascan[i] - zeroAmp;
        }

        // TODO: Узнать у Меркулова, где там ноль и что за амплитуды вообще
        private void Convert6000()
        {
            throw new System.NotImplementedException();
        }

        private void ConvertLinearAmplitudes()
        {
            throw new System.NotImplementedException();
        }
    }
}