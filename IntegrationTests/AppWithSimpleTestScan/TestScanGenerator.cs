#define MexicanHatWavelet
//#define SimpleWavelet

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmpCurvesSummation.Core;
using GprFileService;



namespace AppWithSimpleTestScan
{
    class TestScanGenerator : IFileOpener
    {
        private int _cmpLength = 100;
        private int _ascanLength = 500;
        private double _stepDistance = 0.1;
        private double _stepTime = 1;
        private double _offset = 50;
        private double[] _velocities;
        private double[] _thicknesses;
        private int _numLayers = 4;
        private double _ampForSync = 5;


        public TestScanGenerator()
        {
            _velocities = new double[_numLayers];
            _thicknesses = new double[_numLayers];

            _velocities[0] = 0.3;
            _velocities[1] = 0.1;
            _velocities[2] = 0.1;
            _velocities[3] = 0.05;

            _thicknesses[0] = 0;
            _thicknesses[1] = 0;
            _thicknesses[2] = 2;
            _thicknesses[3] = 1;
        }


        public ICmpScan OpenKrotTxt(string filepath) => GetCmpScan();
        public ICmpScan OpenGeo1(string filepath)
        {
            throw new NotImplementedException();
        }

        public ICmpScan OpenGeo2(string filepath)
        {
            throw new NotImplementedException();
        }

        public ICmpScan OpenGeo(string filepath) => GetCmpScan();


        private ICmpScan GetCmpScan()
        {
            var cmpScan = new CmpScan();

            LoadData(cmpScan);
            //LozaStyleSynchronize(_ampForSync, cmpScan);
            cmpScan.CopyRawDataToProcessed();

            return cmpScan;
        }

        private void LoadData(ICmpScan cmpScan)
        {
            for (int i = 0; i < _cmpLength; i++)
            {
                cmpScan.RawData.Add(new double[_ascanLength]);
                for (int j = 0; j < _ascanLength; j++)
                {
                    cmpScan.RawData[i][j] = SummarizeReflections(i, j);
                }
            }
        }

        private double SummarizeReflections(int i, int j)
        {
            var time = j * _stepTime;
            var distance = i * _stepDistance;
            var reflection = 0.0;

            for (int k = 0; k < _numLayers; k++)
            {
                //                var hodograph = CmpMath.Instance.HodographLineLoza(distance, _thicknesses[k], _velocities[k]);
                var hodograph = HodographLineLoza(distance, _thicknesses, _velocities, k);
                var hodographWithOffset = hodograph + _offset;

#if MexicanHatWavelet
                reflection += MexicanHatWavelet(time, hodographWithOffset) * AttenuationCoef(i, j);
#elif SimpleWavelet
                reflection += SimpleWavelet(time, hodographWithOffset);
#endif
            }

            return reflection;
        }

        private double SimpleWavelet(double time, double offset)
        {
            return Math.Abs(time - offset) <= _stepTime ? 5 : 0;
        }

        private static double MexicanHatWavelet(double time, double offset)
        {
            var ampCoef = 1000;
            var sigma = 9;
            var t = time - offset;
            var part1 = 2 / (Math.Sqrt(3 * sigma) * Math.PI);
            var part2 = 1 - Math.Pow(t / sigma, 2);
            var part3 = Math.Exp(-t * t / (2 * sigma * sigma));
            return ampCoef * part1 * part2 * part3;
        }

        private static double AttenuationCoef(double x, double y) => 1 / ((x / 20 + 1) + (y / 200 + 1));

        private void LozaStyleSynchronize(double ampForSync, ICmpScan cmpScan)
        {
            for (int i = 0; i < _cmpLength; i++)
            {
                var ascan = cmpScan.RawData[i];
                var newStart = 0;
                for (int j = 0; j < _ascanLength; j++)
                {
                    if (Math.Abs(ascan[j]) > ampForSync)
                        break;
                    newStart++;
                }
                for (int j = 0; j < _ascanLength; j++)
                    ascan[j] = j + newStart < _ascanLength ? ascan[j + newStart] : 0;
            }
        }

        public double HodographLineLoza(double distance, double[] depths, double[] velocities, int index)
        {
            if (depths == null || velocities == null)
                throw new ArgumentNullException();
            if (depths.Length != velocities.Length)
                throw new ArgumentException();
            if (index >= velocities.Length || index < 0)
                throw new ArgumentOutOfRangeException();

            double depth = 0;
            for (int i = 0; i <= index; i++)
                depth += depths[i];
            double alpha = 0;
            if (depth == 0)
                alpha =  Double.NaN;
            else
                alpha = Math.Atan(distance / (2*depth));

            double t = 0;
            if (depth != 0)
                for (int i = 0; i <= index; i++)
                    t += 2*depths[i] / velocities[i] / Math.Cos(alpha);
            else
                t += distance / velocities[index];

            t -= distance / CmpMath.SpeedOfLight;

            return t;
        }

    }
}
