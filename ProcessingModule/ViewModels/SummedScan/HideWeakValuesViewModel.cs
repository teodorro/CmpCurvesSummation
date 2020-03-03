using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ProcessingModule.Annotations;
using ProcessingModule.Processing.SummedScan;

namespace ProcessingModule.ViewModels.SummedScan
{
    public class HideWeakValuesViewModel : INotifyPropertyChanged
    {
        public event SumProcessingListChangedHandler ProcessingListChanged;
        private HideWeakValues _processing;

        public double WeakValue
        {
            get => _processing.WeakValue;
            set
            {
                _processing.WeakValue = Math.Round(value, 3);
                OnPropertyChanged(nameof(WeakValue));
                ProcessingListChanged(this, new SumProcessingListChangedEventArgs() { Enabled = true, Processing = _processing });
            }
        }

        public double MaxValue
        {
            get => _processing.MaxValue;
            set
            {
                _processing.MaxValue = Math.Round(value, 3);
                OnPropertyChanged(nameof(MaxValue));
                ProcessingListChanged(this, new SumProcessingListChangedEventArgs() { Enabled = true, Processing = _processing });
            }
        }


        public HideWeakValuesViewModel(HideWeakValues processing)
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