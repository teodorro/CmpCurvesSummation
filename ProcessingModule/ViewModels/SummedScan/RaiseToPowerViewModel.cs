using System.ComponentModel;
using System.Runtime.CompilerServices;
using ProcessingModule.Annotations;
using ProcessingModule.Processing.SummedScan;

namespace ProcessingModule.ViewModels.SummedScan
{
    public class RaiseToPowerViewModel : INotifyPropertyChanged
    {
        public event SumProcessingListChangedHandler ProcessingListChanged;
        private RaiseToPower _processing;

        public double Power
        {
            get => _processing.Power;
            set
            {
                _processing.Power = value;
                OnPropertyChanged(nameof(Power));
                ProcessingListChanged(this, new SumProcessingListChangedEventArgs() { Enabled = true, Processing = _processing });
            }
        }


        public RaiseToPowerViewModel(RaiseToPower processing)
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