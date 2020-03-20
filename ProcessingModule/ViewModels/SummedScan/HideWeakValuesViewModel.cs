using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CmpCurvesSummation.Core;
using ProcessingModule.Annotations;
using ProcessingModule.Processing.SummedScan;

namespace ProcessingModule.ViewModels.SummedScan
{
    public class HideWeakValuesViewModel : INotifyPropertyChanged
    {
        private HideWeakValues _processing = new HideWeakValues();

        public double WeakValue
        {
            get => _processing.WeakValue;
            set
            {
                _processing.WeakValue = Math.Round(value, 3);
                OnPropertyChanged(nameof(WeakValue));
                EventAggregator.Instance.Invoke(this, new SumProcessingValuesChangedEventArgs());
            }
        }

        public double MaxValue
        {
            get => _processing.MaxValue;
            set
            {
                _processing.MaxValue = Math.Round(value, 3);
                OnPropertyChanged(nameof(MaxValue));
                EventAggregator.Instance.Invoke(this, new SumProcessingValuesChangedEventArgs());
            }
        }


        public HideWeakValuesViewModel()
        {
            EventAggregator.Instance.SumProcessingListChanged += OnProcessingListChanged;
        }


        private void OnProcessingListChanged(object obj, CmpCurvesSummation.Core.SumProcessingListChangedEventArgs e)
        {
            if (e.Processing.GetType() != typeof(HideWeakValues))
                return;
            _processing = (HideWeakValues)(e.Enabled == true ? e.Processing : null);
            EventAggregator.Instance.Invoke(this, new SumProcessingValuesChangedEventArgs());
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}