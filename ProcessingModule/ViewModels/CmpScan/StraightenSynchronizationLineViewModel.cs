using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CmpCurvesSummation.Core;
using ProcessingModule.Annotations;
using ProcessingModule.Processing.CmpScan;

namespace ProcessingModule.ViewModels.CmpScan
{
    public class StraightenSynchronizationLineViewModel : INotifyPropertyChanged
    {
        private StraightenSynchronizationLine _processing = new StraightenSynchronizationLine();

        public double MinAmplitudeToCheck
        {
            get => _processing.MinNegativeAmplitudeToBegin;
            set
            {
                if (_processing == null)
                    return;
                _processing.MinNegativeAmplitudeToBegin = Math.Round(value, 3);
                OnPropertyChanged(nameof(MinAmplitudeToCheck));
                EventAggregator.Instance.Invoke(this, new CmpProcessingValuesChangedEventArgs());
            }
        }

        public double MaxAmpToBack
        {
            get => _processing.MinPositiveAmplitudeToStop;
            set
            {
                if (_processing == null)
                    return;
                _processing.MinPositiveAmplitudeToStop = Math.Round(value, 3);
                OnPropertyChanged(nameof(MaxAmpToBack));
                EventAggregator.Instance.Invoke(this, new CmpProcessingValuesChangedEventArgs());
            }
        }

        private double _minAmplitude = -25;
        public double MinAmplitude
        {
            get => Math.Round(_minAmplitude, 2);
            set
            {
                if (_processing == null)
                    return;
                _minAmplitude = value;
                OnPropertyChanged(nameof(MinAmplitude));
            }
        }

        private double _maxAmplitude = 50;
        public double MaxAmplitude
        {
            get => Math.Round(_maxAmplitude, 2);
            set
            {
                if (_processing == null)
                    return;
                _maxAmplitude = value;
                OnPropertyChanged(nameof(MaxAmplitude));
            }
        }


        public StraightenSynchronizationLineViewModel()
        {
            EventAggregator.Instance.CmpProcessingListChanged += OnProcessingListChanged;
            EventAggregator.Instance.FileLoaded += OnFileLoaded;
        }


        private void OnFileLoaded(object obj, FileLoadedEventArgs e)
        {
            MinAmplitudeToCheck = StraightenSynchronizationLine.DefaultValueNeg;
            MaxAmpToBack = StraightenSynchronizationLine.DefaultValuePos;
        }

        private void OnProcessingListChanged(object obj, CmpProcessingListChangedEventArgs e)
        {
            if (e.Processing.GetType() != typeof(StraightenSynchronizationLine))
                return;
            _processing = (StraightenSynchronizationLine)(e.Enabled == true ? e.Processing : null);
            if (_processing != null)
            {
                MinAmplitudeToCheck = _processing.MinNegativeAmplitudeToBegin;
                MaxAmpToBack = _processing.MinPositiveAmplitudeToStop;
            }
            EventAggregator.Instance.Invoke(this, new CmpProcessingValuesChangedEventArgs());
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}