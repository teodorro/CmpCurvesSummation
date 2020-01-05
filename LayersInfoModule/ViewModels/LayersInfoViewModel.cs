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
            Layers.Add(new LayerInfo(){Thickness = Math.Round(e.H, 2), Velocity = Math.Round(e.V, 2)});
        }
    }


    public class LayerInfo
    {
        public double Thickness { get; set; }
        public double Velocity { get; set; }

    }
}