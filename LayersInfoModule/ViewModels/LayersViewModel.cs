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
            var newLayer = new LayerInfo(e.Time, e.Velocity);
            Layers.Add(newLayer);
            SortLayers();

        }

        private void SortLayers()
        {
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


    /// <summary>
    /// Compare different layers to make it organized according to the depth
    /// </summary>
    public class LayerComparer : IComparer<LayerInfo>
    {
        public int Compare(LayerInfo x, LayerInfo y)
        {
            return Convert.ToInt32((x.Time - y.Time) * 100);
        }
    }


    /// <summary>
    /// info about layer which is used as a row in the table of layers
    /// </summary>
    public class LayerInfo
    {
        public LayerInfo(double time, double avgVelocity)
        {
            Time = time;
            AvgVelocity = avgVelocity;
        }

        [DisplayName("Время, нс")]
        public double Time { get; set; }
        [DisplayName("Глубина, м")]
        public double Height => Math.Round(Time * AvgVelocity, 2);
        [DisplayName("Толщина, м")]
        public double Thickness { get; }
        [DisplayName("Средняя скорость, м/нс")]
        public double AvgVelocity { get; }
        [DisplayName("Скорость в слое, м/нс")]
        public double LayerVelocity { get; }
        [DisplayName("Диэл. пр-ть слоя")]
        public double Permittivity => Math.Round(CmpMath.Instance.Permittivity(AvgVelocity), 2);
    }
}