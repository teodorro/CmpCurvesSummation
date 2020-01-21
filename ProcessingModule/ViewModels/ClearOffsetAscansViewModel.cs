using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ProcessingModule.Annotations;

namespace ProcessingModule.ViewModels
{
    public class ClearOffsetAscansViewModel : INotifyPropertyChanged
    {
        public event ProcessingListChangedHandler ProcessingListChanged;

        private int _numberOfOffsetAscans = 5;
        public int NumberOfOffsetAscans {
            get => _numberOfOffsetAscans;
            set
            {
                _numberOfOffsetAscans = value;
                OnPropertyChanged(nameof(NumberOfOffsetAscans));
                ProcessingListChanged(this, new ProcessingListChangedEventArgs());
            }
        }

        public ObservableCollection<int> Numbers { get; set; } = new ObservableCollection<int>();


        public ClearOffsetAscansViewModel(int numberOfOffsetAscans)
        {
            _numberOfOffsetAscans = numberOfOffsetAscans;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}