using System;
using System.Collections.ObjectModel;
using CmpCurvesSummation.Core;

namespace LayersInfoModule.ViewModels
{
    public delegate void DeleteLayerHander(object obj, DeleteLayerEventsArgs e);

    public class LayersInfoViewModel
    {

        public event DeleteLayerHander DeleteClick;

        public ObservableCollection<LayerInfo> Layers { get; } = new ObservableCollection<LayerInfo>();

        public void OnHodographDrawClick(object sender, HodographDrawVTClickEventArgs e)
        {
            Layers.Add(new LayerInfo(0, 0, e.Velocity){Time = e.Time});
        }

        public void OnDeleteRowClick(object sender, DeleteLayerEventsArgs e)
        {
            DeleteClick.Invoke(sender, e);
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

        public double Time { get; set; }
        public double Height { get; }
        public double Thickness { get; }
        public double Velocity { get; }
        public double Permittivity => Math.Round(CmpMath.Instance.Permittivity(Velocity), 2);
    }
}