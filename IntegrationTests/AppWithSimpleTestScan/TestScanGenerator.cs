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
//        private double _offset = 50;

        /// <summary>
        /// Using radiovelocity, not the pure
        /// </summary>
        private double[] _velocities;
        private double[] _heights;
        private int _numLayers = 3;
        private double _ampForSync = 5;


        public TestScanGenerator()
        {
            _velocities = new double[_numLayers];
            _heights = new double[_numLayers];

            _velocities[0] = 0.15;
            _velocities[1] = 0.05;
            _velocities[2] = 0.05;
            //_velocities[3] = 0.04;

            _heights[0] = 0;
            _heights[1] = 0;
            _heights[2] = 2;
//            _heights[3] = 5;
        }

        
        public ICmpScan OpenKrotTxt(string filepath) => GetCmpScan();

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
//            var hodograph1 = CmpMath.Instance.HodographLineLoza(distance, _heights[0], _velocities[0]);
//            var reflection1 = MexicanHatWavelet(time, hodograph1) * AttenuationCoef(i, j);
//            var reflection1 = SimpleWavelet(time, hodograph1) ;

            for (int k = 0; k < _numLayers; k++)
            {
                var hodograph = CmpMath.Instance.HodographLineLoza(distance, _heights[k], _velocities[k]);
                reflection += SimpleWavelet(time, hodograph);
//                reflection += MexicanHatWavelet(time, hodograph) * AttenuationCoef(i, j);
                //                asd[i] = CmpMath.Instance.HodographLineLoza(distance, _heights[2], _velocities[2]);
            }

            //            var hodograph2 = HodographLine(distance, 1);
            //            var reflection2 = MexicanHatWavelet(time, hodograph2) * AttenuationCoef(i, j);

            //            return reflection1;// + reflection2;
            return reflection;
        }

//        private double[] asd = new double[100];

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

//        private double HodographLine(double d, int indexLayer)
//        {
//            if (indexLayer >= _numLayers)
//                throw new IndexOutOfRangeException("слоев меньше");
//
//            var part1 = 1 / _velocities[indexLayer];
//            var part2 = Math.Sqrt(Math.Pow(_heights[indexLayer], 2) + Math.Pow(d, 2) / 4);
//            return part1 * part2;
//        }

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

    }
}
