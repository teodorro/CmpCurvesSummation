using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CmpCurvesSummation.Core;

namespace GprFileService
{
    /// <summary>
    /// To open all types of files
    /// </summary>
    public interface IFileOpener
    {
        ICmpScan OpenKrotTxt(string filepath);
        ICmpScan OpenGeo(string filepath);
    }


    public class FileOpener : IFileOpener
    {
        public ICmpScan OpenKrotTxt(string filepath)
        {
            var lines = File.ReadAllLines(filepath);
            if (lines.Length < 2)
                throw new FileLoadException("Ошибка при загрузке файла. Количество строк < 2", filepath);

            try
            {
                var krotFileData = ExtractDataFromKrotTxtFile(lines);
                var cmpScan = CreateCmpScanFromKrotData(krotFileData);
                RemoveEmptyAscans(cmpScan);
                cmpScan.CopyRawDataToProcessed();

                return cmpScan;
            }
            catch (Exception ex)
            {
                throw new FileLoadException("Ошибка при загрузке файла " + ex.Message, filepath);
            }
        }

        private ICmpScan CreateCmpScanFromKrotData(List<double[]> krotFileData)
        {
            var cmpScan = new CmpScan { StepTime = krotFileData[1][1] - krotFileData[0][1] };
            foreach (var piece in krotFileData)
            {
                var curAscanNum = Convert.ToInt32(piece[0]);
                if (curAscanNum == cmpScan.RawData.Count)
                {
                    var ascanLengthDimensionless = krotFileData.Count(x => x[0] == curAscanNum);
                    cmpScan.RawData.Add(new double[ascanLengthDimensionless]);
                }
                var curAscanPoint = Convert.ToInt32(piece[1] / cmpScan.StepTime);
                cmpScan.RawData[curAscanNum][curAscanPoint] = piece[2];
            }

            return cmpScan;
        }

        private List<double[]> ExtractDataFromKrotTxtFile(string[] lines)
        {
            var krotFileData = new List<double[]>();
            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i].Substring(0, lines[i].Length - 1);
                var numAscan = Convert.ToDouble(line.Substring(0, line.IndexOf(";")));
                var time = Convert.ToDouble(
                    line.Substring(line.IndexOf(";") + 1, line.LastIndexOf(";") - line.IndexOf(";") - 1));
                var amplitude = Convert.ToDouble(line.Substring(line.LastIndexOf(";") + 1, line.Length - line.LastIndexOf(";") - 1));
                krotFileData.Add(new double[] { numAscan, time, amplitude });
            }

            return krotFileData;
        }

        /// <summary>
        /// Krot software create convert data to a list of 100 ascan packs. 
        /// If the length of isn't equal to *00, some empty ascans are appended ot make it to be equal.
        /// 0 is not minimum amplitude but it signals that it's an empty ascan
        /// </summary>
        private void RemoveEmptyAscans(ICmpScan cmpScan)
        {
            int emptyAscanIndex = 0;
            for (int i = 0; i < cmpScan.RawData.Count; i++)
            {
                if (cmpScan.RawData[i].All(x => x == 0))
                {
                    emptyAscanIndex = i;
                    break;
                }
            }
            cmpScan.RawData.RemoveRange(emptyAscanIndex, cmpScan.RawData.Count - emptyAscanIndex);
        }

        public ICmpScan OpenGeo(string filepath)
        {
            throw new System.NotImplementedException();
        }
    }
}