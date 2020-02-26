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
        ICmpScan OpenGeo1(string filepath);
        ICmpScan OpenGeo2(string filepath);
        ICmpScan OpenGem(string filepath);
    }



    public interface IFileTypeOpener
    {
        ICmpScan OpenFile(string filepath);
    }



    public class FileOpener : IFileOpener
    {
        public ICmpScan OpenKrotTxt(string filepath)
        {
            return new TxtKrotFileOpener().OpenFile(filepath);
        }

        public ICmpScan OpenGeo1(string filepath)
        {
            return new GeoFileOpener1().OpenFile(filepath);
        }

        public ICmpScan OpenGeo2(string filepath)
        {
            return new GeoFileOpener2().OpenFile(filepath);
        }

        public ICmpScan OpenGem(string filepath)
        {
            return new GemFileOpener().OpenFile(filepath);
        }
    }
}