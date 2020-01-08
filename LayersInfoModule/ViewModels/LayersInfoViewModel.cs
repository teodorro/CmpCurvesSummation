using System;
using System.Collections.ObjectModel;
using CmpCurvesSummation.Core;

namespace LayersInfoModule.ViewModels
{
    public class LayersInfoViewModel
    {
        public ObservableCollection<LayerInfo> Layers { get; } = new ObservableCollection<LayerInfo>();

        public void OnHodographDrawClick(object sender, HodographDrawClickEventArgs e)
        {
            Layers.Add(new LayerInfo(Math.Round(e.H, 2), Math.Round(e.H, 2), Math.Round(e.V, 2)));
        }
    }


    public class LayerInfo
    {
        public LayerInfo(double height, double thickness, double velocity)
        {
            Height = height;
            Thickness = thickness;
            Velocity = velocity;
        }

        public double Height { get; }
        public double Thickness { get; }
        public double Velocity { get; }
        public double Permittivity => Math.Round(CmpMath.Instance.Permittivity(Velocity), 2);
    }
}