﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ProcessingModule.Annotations;
using ProcessingModule.Processing;

namespace ProcessingModule.ViewModels
{
    public class StraightenSynchronizationLine2ViewModel : INotifyPropertyChanged
    {
        public event ProcessingListChangedHandler ProcessingListChanged;

        private StraightenSynchronizationLine2 _processing;

        public double MinAmplitudeToCheck
        {
            get => _processing.MinNegativeAmplitudeToBegin;
            set
            {
                _processing.MinNegativeAmplitudeToBegin = value;
                OnPropertyChanged(nameof(MinAmplitudeToCheck));
                ProcessingListChanged(this, new ProcessingListChangedEventArgs() { Enabled = true, Processing = _processing });
            }
        }

        public double MaxAmpToBack
        {
            get => _processing.MinPositiveAmplitudeToStop;
            set
            {
                _processing.MinPositiveAmplitudeToStop = value;
                OnPropertyChanged(nameof(MaxAmpToBack));
                ProcessingListChanged(this, new ProcessingListChangedEventArgs() { Enabled = true, Processing = _processing });
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

        private double _maxAmplitude = 100;
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