using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using CmpCurvesSummation.Core;

namespace LayersInfoModule.ViewModels
{
    public delegate void DeleteLayerHander(object obj, DeleteLayerEventsArgs e);

    public class LayersViewModel
    {
        public event DeleteLayerHander DeleteClick;

        public ObservableCollection<LayerInfo> Layers { get; } = new ObservableCollection<LayerInfo>();


        public void OnHodographDrawClick(object sender, HodographDrawVTClickEventArgs e)
        {
            Layers.Add(new LayerInfo(e.Time, e.Velocity));
            var sorted = Layers.OrderBy(x => x.Time).ToList();
            Layers.Clear();
            foreach (var layer in sorted)
                Layers.Add(layer);
        }

        public void OnDeleteRowClick(object sender, DeleteLayerEventsArgs e)
        {
            DeleteClick?.Invoke(sender, e);
        }
    }

    public class LayerComparer : IComparer<LayerInfo>
    {
        public int Compare(LayerInfo x, LayerInfo y)
        {
            return Convert.ToInt32((x.Time - y.Time) * 100);
        }
    }


    public class LayerInfo
    {
        public LayerInfo(double time, double velocity)
        {
            Time = time;
            Velocity = velocity;
        }

        [DisplayName("Время")]
        public double Time { get; set; }
        [DisplayName("Глубина")]
        public double Height { get; }
        [DisplayName("Толщина")]
        public double Thickness { get; }
        [DisplayName("Скорость")]
        public double Velocity { get; }
        [DisplayName("Диэл. пр-ть")]
        public double Permittivity => Math.Round(CmpMath.Instance.Permittivity(Velocity), 2);
    }
}