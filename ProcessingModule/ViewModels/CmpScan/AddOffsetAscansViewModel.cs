using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using CmpCurvesSummation.Core;
using ProcessingModule.Annotations;
using ProcessingModule.Processing.CmpScan;

namespace ProcessingModule.ViewModels
{
    public class AddOffsetAscansViewModel : INotifyPropertyChanged
    {
        private AddOffsetAscans _processing;

        public int NumberOfOffsetAscans
        {
            get => Convert.ToInt32(_processing?.NumberOfAscans);
            set
            {
                _processing.NumberOfAscans = value;
                OnPropertyChanged(nameof(NumberOfOffsetAscans));
                EventAggregator.Instance.Invoke(this, new CmpProcessingValuesChangedEventArgs());
            }
        }


        public AddOffsetAscansViewModel()
        {
            EventAggregator.Instance.CmpProcessingListChanged += OnProcessingListChanged;
        }


        private void OnProcessingListChanged(object obj, CmpProcessingListChangedEventArgs e)
        {
            if (e.Processing.GetType() != typeof(AddOffsetAscans))
                return;
            _processing = (AddOffsetAscans) (e.Enabled == true ? e.Processing : null);
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