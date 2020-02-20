using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ProcessingModule.Annotations;
using ProcessingModule.Processing;
using ProcessingModule.Processing.CmpScan;

namespace ProcessingModule.ViewModels
{
    public class StraightenSynchronizationLine2ViewModel : INotifyPropertyChanged
    {
        public event CmpProcessingListChangedHandler ProcessingListChanged;

        private StraightenSynchronizationLine2 _processing;

        public double MinAmplitudeToCheck
        {
            get => _processing.MinNegativeAmplitudeToBegin;
            set
            {
                _processing.MinNegativeAmplitudeToBegin = Math.Round(value, 3);
                OnPropertyChanged(nameof(MinAmplitudeToCheck));
                ProcessingListChanged(this, new CmpProcessingListChangedEventArgs() { Enabled = true, Processing = _processing });
            }
        }

        public double MaxAmpToBack
        {
            get => _processing.MinPositiveAmplitudeToStop;
            set
            {
                _processing.MinPositiveAmplitudeToStop = Math.Round(value, 3);
                OnPropertyChanged(nameof(MaxAmpToBack));
                ProcessingListChanged(this, new CmpProcessingListChangedEventArgs() { Enabled = true, Processing = _processing });
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


        public StraightenSynchronizationLine2ViewModel(StraightenSynchronizationLine2 processing)
        {
            _processing = processing;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}