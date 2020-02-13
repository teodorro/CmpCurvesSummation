using System.ComponentModel;
using System.Runtime.CompilerServices;
using CmpCurvesSummation.Core;
using LayersInfoModule.Annotations;

namespace SummedScanModule.ViewModels
{
    public class SummedScanOptionsViewModel : INotifyPropertyChanged
    {
        public event AutoCorrectionCheckHander AutoCorrectionClick;
        public event AlphaChangedHandler AlphaChanged;
        public event HalfWaveSizeChangedHandler HalfWaveSizeChanged;


        private bool _autoCorrection;
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

        private byte _alpha;
        public byte Alpha
        {
            get => _alpha;
            set
            {
                _alpha = value;
                OnPropertyChanged(nameof(Alpha));
                AlphaChanged?.Invoke(this, new AlphaChangedEventArgs(_alpha));
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
                HalfWaveSizeChanged?.Invoke(this, new HalfWaveSizeChangedEventArgs(_halfWaveSize));
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