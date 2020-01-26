using System.ComponentModel;
using System.Runtime.CompilerServices;
using ProcessingModule.Annotations;
using ProcessingModule.Processing;

namespace ProcessingModule.ViewModels
{
    public class AddOffsetAscansViewModel : INotifyPropertyChanged
    {
        public event ProcessingListChangedHandler ProcessingListChanged;

        private AddOffsetAscans _processing;
        public int NumberOfOffsetAscans
        {
            get => _processing.NumberOfAscans;
            set
            {
                _processing.NumberOfAscans = value;
                OnPropertyChanged(nameof(NumberOfOffsetAscans));
                ProcessingListChanged(this, new ProcessingListChangedEventArgs() { Enabled = true, Processing = _processing });
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