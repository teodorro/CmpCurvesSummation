using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using CmpCurvesSummation.Core;
using LayersInfoModule.Annotations;

namespace LayersInfoModule.ViewModels
{
    public class LayersViewModel : INotifyPropertyChanged
    {
        public event AutoCorrectionCheckHander AutoCorrectionClick;

        public ObservableCollection<LayerInfo> Layers { get; } = new ObservableCollection<LayerInfo>();

        private bool _autoCorrection;
        private ISummedScanVT _summedScan;

        public bool AutoCorrection
        {
            get => _autoCorrection;
            set
            {
                _autoCorrection = value;
                OnPropertyChanged(nameof(AutoCorrection));
                AutoCorrectionClick?.Invoke(this, new AutoCorrectionCheckEventArgs(_autoCorrection));
            }
        }


        public void OnDeleteRowClick(object sender, DeleteLayerEventArgs e)
        {
            _summedScan.RemoveLayersAround(e.Velocity, e.Time);
        }

        public void OnFileLoaded(object sender, FileLoadedEventArgs e)
        {
            Layers.Clear();
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void OnRawCmpDataProcessed(object obj, RawCmpProcessedEventArgs e)
        {
            Layers.Clear();
        }

        public void OnStepDistanceChanged(object obj, StepDistanceEventArgs e)
        {
            Layers.Clear();
        }

        public void OnStepTimeChanged(object obj, StepTimeEventArgs e)
        {
            Layers.Clear();
        }

        public void OnSummationFinished(object obj, SummationFinishedEventArgs e)
        {
            _summedScan = e.SummedScan;
            _summedScan.RefreshLayers += OnRefreshLayers;
            Layers.Clear();
        }

        private void OnRefreshLayers(object o, RefreshLayersEventArgs e)
        {
            Layers.Clear();
            LayerInfo prevLayerInfo = null;
            foreach (var layer in e.Layers)
            {
                var avgVelocity = layer.Item1;
                var time = layer.Item2;
                var layerInfo = new LayerInfo(time, avgVelocity, prevLayerInfo);
                Layers.Add(layerInfo);
                prevLayerInfo = layerInfo;
            }
        }

        public void OnTimeOffsetChanged(object obj, TimeOffsetChangedEventArgs e)
        {
            Layers.Clear();
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



    public class SelectedRowToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (int)value == 1;
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) { throw new NotImplementedException();}
    }
}