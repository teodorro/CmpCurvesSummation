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
        public ObservableCollection<LayerInfo> Layers { get; } = new ObservableCollection<LayerInfo>();

        private ISummedScanVT _summedScan;


        public LayersViewModel()
        {
            EventAggregator.Instance.CmpScanParametersChanged += OnTimeOffsetChanged;
            EventAggregator.Instance.FileLoaded += OnFileLoaded;
            EventAggregator.Instance.CmpDataProcessed += OnCmpDataProcessed;
            EventAggregator.Instance.CmpScanParametersChanged += OnCmpScanParametersChanged;
            EventAggregator.Instance.SummationFinished += OnSummationFinished;
        }

        private void OnCmpScanParametersChanged(object o, CmpScanParametersChangedEventArgs cmpScanParametersChangedEventArgs)
        {
            Layers.Clear();
        }
        
        internal void OnDeleteRowClick(object sender, DeleteLayerEventArgs e)
        {
            _summedScan.RemoveLayersAround(e.Velocity, e.Time);
        }

        private void OnFileLoaded(object sender, FileLoadedEventArgs e)
        {
            Layers.Clear();
        }

        private void OnCmpDataProcessed(object obj, CmpDataProcessedEventArgs e)
        {
            Layers.Clear();
        }

        private void OnSummationFinished(object sender, SummationFinishedEventArgs e)
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

        private void OnTimeOffsetChanged(object obj, CmpScanParametersChangedEventArgs e)
        {
            Layers.Clear();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }


    public class DeleteLayerEventArgs : EventArgs
    {
        public DeleteLayerEventArgs(double velocity, double time)
        {
            Velocity = velocity;
            Time = time;
        }
    
        public double Velocity { get; }
        public double Time { get; }
    }


    public class SelectedRowToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (int)value == 1;
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) { throw new NotImplementedException();}
    }
}