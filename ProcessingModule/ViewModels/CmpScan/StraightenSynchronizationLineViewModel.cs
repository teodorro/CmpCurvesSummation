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
                _maxAmplitude = value;
                OnPropertyChanged(nameof(MaxAmplitude));
            }
        }


        public StraightenSynchronizationLineViewModel()
        {
            EventAggregator.Instance.CmpProcessingListChanged += OnProcessingListChanged;
        }


        private void OnProcessingListChanged(object obj, CmpProcessingListChangedEventArgs e)
        {
            if (e.Processing.GetType() != typeof(StraightenSynchronizationLine))
                return;
            _processing = (StraightenSynchronizationLine)(e.Enabled == true ? e.Processing : null);
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