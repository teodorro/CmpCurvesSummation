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
        [DisplayName("Время, нс")]
        public double Time
        {
            get => Math.Round(_time, 1);
            set
            {
                _time = value;
                OnPropertyChanged(nameof(Time));
                OnPropertyChanged(nameof(Depth));
            }
        }

        [DisplayName("Глубина, м")]
        public double Depth => Math.Round(CmpMath.Instance.Depth(AvgVelocity, Time), 2);

        private double _avgVelocity;
        [DisplayName("Средняя скорость, м/нс")]
        public double AvgVelocity
        {
            get => _avgVelocity;
            set
            {
                _avgVelocity = value;
                OnPropertyChanged(nameof(AvgVelocity));
                OnPropertyChanged(nameof(Depth));
            }
        }


        private double _thickness;
        [DisplayName("Толщина, м")]
        public double Thickness
        {
            get => _thickness;
            set
            {
                _thickness = value;
                OnPropertyChanged(nameof(Thickness));
            }
        }

        public bool IsLayerThicknessOk => Thickness > 0;

        private double _layerVelocity;
        [DisplayName("Скорость в слое, м/нс")]
        public double LayerVelocity
        {
            get => _layerVelocity;
            set
            {
                _layerVelocity = value;
                OnPropertyChanged(nameof(LayerVelocity));
                OnPropertyChanged(nameof(LayerPermittivity));
            }
        }

        public bool IsLayerVelocityOk => LayerVelocity >= SummedScanVT.AbsoluteMinVelocity && LayerVelocity <= CmpMath.SpeedOfLight / 2;

        [DisplayName("Диэл. пр-ть слоя")]
        public double LayerPermittivity => Math.Round(CmpMath.Instance.Permittivity(LayerVelocity), 2);

        public bool IsLayerPermittivityOk => LayerPermittivity >= 1 && LayerPermittivity <= 100;


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}