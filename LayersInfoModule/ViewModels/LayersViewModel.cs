using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using CmpCurvesSummation.Core;
using LayersInfoModule.Annotations;

namespace LayersInfoModule.ViewModels
{
    public delegate void DeleteLayerHander(object obj, DeleteLayerEventArgs e);


    public class LayersViewModel : INotifyPropertyChanged
    {
        public event DeleteLayerHander DeleteClick;

        public ObservableCollection<LayerInfo> Layers { get; } = new ObservableCollection<LayerInfo>();
        
        
        public void OnHodographDrawClick(object sender, HodographDrawVTClickEventArgs e)
        {

            var newLayer = new LayerInfo(e.Time, e.Velocity, Layers.LastOrDefault(x => x.Time < e.Time));
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

        public void OnDeleteRowClick(object sender, DeleteLayerEventArgs e)
        {
            Layers.Remove(Layers.First(x => x.Time == e.Time && x.AvgVelocity == e.Velocity));
            DeleteClick?.Invoke(sender, e);
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }


    /// <summary>
    /// info about layer which is used as a row in the table of layers
    /// </summary>
    public class LayerInfo
    {
        public LayerInfo(double time, double avgVelocity, LayerInfo prevLayer)
        {
            Time = time;
            AvgVelocity = avgVelocity;
            if (prevLayer == null)
            {
                Thickness = Depth;
                LayerVelocity = AvgVelocity;
            }
            else
            {
                var depth = CmpMath.Instance.Depth(avgVelocity, time);
                Thickness = Math.Round(CmpMath.Instance.LayerThickness(depth, prevLayer.Depth), 2);
                LayerVelocity = Math.Round(CmpMath.Instance.LayerVelocity(time, 0, depth, avgVelocity, prevLayer.Depth, prevLayer.AvgVelocity), 3);

            }
        }

        [DisplayName("Время, нс")]
        public double Time { get; set; }
        [DisplayName("Глубина, м")]
        public double Depth => Math.Round(CmpMath.Instance.Depth(AvgVelocity, Time), 2);
        [DisplayName("Средняя скорость, м/нс")]
        public double AvgVelocity { get; }
        [DisplayName("Толщина, м")]
        public double Thickness { get; }
        [DisplayName("Скорость в слое, м/нс")]
        public double LayerVelocity { get; }
        [DisplayName("Диэл. пр-ть слоя")]
        public double Permittivity => Math.Round(CmpMath.Instance.Permittivity(LayerVelocity), 2);
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
}