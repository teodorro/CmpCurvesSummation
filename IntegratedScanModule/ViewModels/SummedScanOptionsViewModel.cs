using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CmpCurvesSummation.Core;
using LayersInfoModule.Annotations;

namespace SummedScanModule.ViewModels
{
    public class SummedScanOptionsViewModel : INotifyPropertyChanged
    {
        private bool _autoCorrection;
        public bool AutoCorrection
        {
            get => _autoCorrection;
            set
            {
                _autoCorrection = value;
                OnPropertyChanged(nameof(AutoCorrection));
                EventAggregator.Instance.Invoke(this, new SumScanOptionsChangedEventArgs(_autoCorrection, _alpha, _halfWaveSize));
            }
        }

        private byte _alpha;
        public byte Alpha
        {
            get => _alpha;
            set
            {
                _alpha = value;
                OnPropertyChanged(nameof(Alpha));
                EventAggregator.Instance.Invoke(this, new SumScanOptionsChangedEventArgs(_autoCorrection, _alpha, _halfWaveSize));
            }
        }

        private int _halfWaveSize = 5;
        public int HalfWaveSize
        {
            get => _halfWaveSize;
            set
            {
                _halfWaveSize = value;
                OnPropertyChanged(nameof(HalfWaveSize));
                EventAggregator.Instance.Invoke(this, new SumScanOptionsChangedEventArgs(_autoCorrection, _alpha, _halfWaveSize));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}