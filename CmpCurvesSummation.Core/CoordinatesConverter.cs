namespace CmpCurvesSummation.Core
{
    public interface ICoordinatesConverter
    {
        double ScreenLength { get; set; }
        double ScreenHeight { get; set; }
        double X { get; set; }
        double H { get; set; }
        double Permittivity { get; set; }
        double CmpScanX { get; set; }
        double CmpScanY { get; set; }
        double IntegratedScanX { get; set; }
        double IntegratedScanY { get; set; }

    }


    public class CoordinatesConverter : ICoordinatesConverter
    {
        public double ScreenLength { get; set; }
        public double ScreenHeight { get; set; }
        public double X { get; set; }
        public double H { get; set; }
        public double Permittivity { get; set; }
        public double CmpScanX { get; set; }
        public double CmpScanY { get; set; }
        public double IntegratedScanX { get; set; }
        public double IntegratedScanY { get; set; }
    }

}