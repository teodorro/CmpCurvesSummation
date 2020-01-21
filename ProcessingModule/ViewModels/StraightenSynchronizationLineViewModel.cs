using System.ComponentModel;
using System.Runtime.CompilerServices;
using ProcessingModule.Annotations;
using ProcessingModule.Processing;

namespace ProcessingModule.ViewModels
{
    public class StraightenSynchronizationLineViewModel : INotifyPropertyChanged
    {
        public event ProcessingListChangedHandler ProcessingListChanged;

        private StraightenSynchronizationLine _processing;

        public double MinAmplitudeToCheck
        {
            get => _processing.MinAmplitudeToCheck;
            set
            {
                _processing.MinAmplitudeToCheck = value;
                OnPropertyChanged(nameof(MinAmplitudeToCheck));
                ProcessingListChanged(this, new ProcessingListChangedEventArgs() { Enabled = true, Processing = _processing });
            }
        }

        public StraightenSynchronizationLineViewModel(StraightenSynchronizationLine processing)
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