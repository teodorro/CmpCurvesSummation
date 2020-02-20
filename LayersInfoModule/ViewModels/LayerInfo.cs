using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CmpCurvesSummation.Core;
using LayersInfoModule.Annotations;

namespace LayersInfoModule.ViewModels
{
    /// <summary>
    /// info about layer which is used as a row in the table of layers
    /// </summary>
    public class LayerInfo : INotifyPropertyChanged
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

        private double _time;
        public double Time
        {
            get => Math.Round(_time, 1);
            set
            {
                _time = value;
                OnPropertyChanged(nameof(Time));
                OnPropertyChanged(nameof(Depth));
                OnPropertyChanged(nameof(TimeDisplay));
                OnPropertyChanged(nameof(DepthDisplay));
            }
        }
        public double TimeDisplay => Math.Round(Time, 1);

        public double Depth => CmpMath.Instance.Depth(AvgVelocity, Time);
        public double DepthDisplay => Math.Round(CmpMath.Instance.Depth(AvgVelocity, Time), 2);

        private double _avgVelocity;
        public double AvgVelocity
        {
            get => _avgVelocity;
            set
            {
                _avgVelocity = value;
                OnPropertyChanged(nameof(AvgVelocity));
                OnPropertyChanged(nameof(Depth));
                OnPropertyChanged(nameof(AvgVelocityDisplay));
                OnPropertyChanged(nameof(DepthDisplay));
            }
        }
        public double AvgVelocityDisplay => Math.Round(AvgVelocity * 100, 2);
        
        private double _thickness;
        public double Thickness
        {
            get => _thickness;
            set
            {
                _thickness = value;
                OnPropertyChanged(nameof(Thickness));
                OnPropertyChanged(nameof(ThicknessDisplay));
            }
        }
        public double ThicknessDisplay => Math.Round(Thickness, 2);

        public bool IsLayerThicknessOk => Thickness > 0;

        private double _layerVelocity;
        public double LayerVelocity
        {
            get => _layerVelocity;
            set
            {
                _layerVelocity = value;
                OnPropertyChanged(nameof(LayerVelocity));
                OnPropertyChanged(nameof(LayerPermittivity));
                OnPropertyChanged(nameof(LayerVelocityDisplay));
                OnPropertyChanged(nameof(LayerPermittivityDisplay));
            }
        }
        public double LayerVelocityDisplay => Math.Round(LayerVelocity * 100, 2);

        public bool IsLayerVelocityOk => LayerVelocity >= CmpMath.Instance.WaterVelocity && LayerVelocity <= CmpMath.AirVelocity;

        public double LayerPermittivity => CmpMath.Instance.Permittivity(LayerVelocity);
        public double LayerPermittivityDisplay => Math.Round(CmpMath.Instance.Permittivity(LayerVelocity), 2);

        public bool IsLayerPermittivityOk => LayerPermittivity >= CmpMath.AirPermittivity && LayerPermittivity <= CmpMath.WaterPermittivity;


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}