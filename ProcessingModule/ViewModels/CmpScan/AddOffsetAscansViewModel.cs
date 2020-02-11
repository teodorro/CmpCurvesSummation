using System.ComponentModel;
using System.Runtime.CompilerServices;
using ProcessingModule.Annotations;
using ProcessingModule.Processing.CmpScan;

namespace ProcessingModule.ViewModels
{
    public class AddOffsetAscansViewModel : INotifyPropertyChanged
    {
        public event CmpProcessingListChangedHandler ProcessingListChanged;

        private AddOffsetAscans _processing;

        public int NumberOfOffsetAscans
        {
            get => _processing.NumberOfAscans;
            set
            {
                _processing.NumberOfAscans = value;
                OnPropertyChanged(nameof(NumberOfOffsetAscans));
                ProcessingListChanged(this, new CmpProcessingListChangedEventArgs() { Enabled = true, Processing = _processing });
            }
        }


        public AddOffsetAscansViewModel(AddOffsetAscans processing)
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