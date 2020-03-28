using System.ComponentModel;
using System.Runtime.CompilerServices;
using CmpCurvesSummation.Core;
using ProcessingModule.Annotations;
using ProcessingModule.Processing.CmpScan;

namespace ProcessingModule.ViewModels.CmpScan
{
    public class RemoveRightAscansViewModel : INotifyPropertyChanged
    {
        private RemoveRightAscans _processing = new RemoveRightAscans();

        public int NumberOfAscans
        {
            get => _processing.NumberOfAscans;
            set
            {
                if (_processing == null)
                    return;
                _processing.NumberOfAscans = value;
                OnPropertyChanged(nameof(NumberOfAscans));
                EventAggregator.Instance.Invoke(this, new CmpProcessingValuesChangedEventArgs());
            }
        }

        public int MaximumNumberOfAscans
        {
            get => _processing.MaximumNumberOfAscans;
            set
            {
                if (_processing == null)
                    return;
                _processing.MaximumNumberOfAscans = value;
                OnPropertyChanged(nameof(MaximumNumberOfAscans));
            }
        }


        public RemoveRightAscansViewModel()
        {
            EventAggregator.Instance.CmpProcessingListChanged += OnProcessingListChanged;
        }


        private void OnProcessingListChanged(object obj, CmpProcessingListChangedEventArgs e)
        {
            if (e.Processing.GetType() != typeof(RemoveRightAscans))
                return;
            _processing = (RemoveRightAscans)(e.Enabled == true ? e.Processing : null);
            if (_processing != null)
            {
                NumberOfAscans = _processing.NumberOfAscans;
                MaximumNumberOfAscans = _processing.MaximumNumberOfAscans;
            }

            EventAggregator.Instance.Invoke(this, new CmpProcessingValuesChangedEventArgs());
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}