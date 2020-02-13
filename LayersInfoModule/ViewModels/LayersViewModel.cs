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
        public event AlphaChangedHandler AlphaChanged;
        public event HalfWaveSizeChangedHandler HalfWaveSizeChanged;

        public ObservableCollection<LayerInfo> Layers { get; } = new ObservableCollection<LayerInfo>();

        private ISummedScanVT _summedScan;


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

        public void OnCmpDataProcessed(object obj, CmpProcessedEventArgs e)
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

        public void OnSummationFinished(object sender, SummationFinishedEventArgs e)
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


    public class SelectedRowToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (int)value == 1;
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) { throw new NotImplementedException();}
    }
}