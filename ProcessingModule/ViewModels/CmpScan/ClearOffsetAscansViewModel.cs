using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CmpCurvesSummation.Core;
using ProcessingModule.Annotations;
using ProcessingModule.Processing;
using ProcessingModule.Processing.CmpScan;

namespace ProcessingModule.ViewModels
{
    public class ClearOffsetAscansViewModel : INotifyPropertyChanged
    {
        private ClearOffsetAscans _processing = new ClearOffsetAscans();

        public int NumberOfOffsetAscans {
            get => _processing.NumberOfAscans;
            set
            {
                _processing.NumberOfAscans = value;
                OnPropertyChanged(nameof(NumberOfOffsetAscans));
                EventAggregator.Instance.Invoke(this, new CmpProcessingValuesChangedEventArgs());
            }
        }


        public ClearOffsetAscansViewModel()
        {
            EventAggregator.Instance.CmpProcessingListChanged += OnProcessingListChanged;
        }


        private void OnProcessingListChanged(object obj, CmpProcessingListChangedEventArgs e)
        {
            if (e.Processing.GetType() != typeof(ClearOffsetAscans))
                return;
            _processing = (ClearOffsetAscans)(e.Enabled == true ? e.Processing : null);
            EventAggregator.Instance.Invoke(this, new CmpProcessingValuesChangedEventArgs());
        }

//        public void Invoke(bool visible)
//        {
//            EventAggregator.Instance.Invoke(this,
//                new CmpProcessingListChangedEventArgs() { Enabled = visible, Processing = _processing });
//        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}