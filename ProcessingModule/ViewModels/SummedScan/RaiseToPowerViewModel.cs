using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CmpCurvesSummation.Core;
using ProcessingModule.Annotations;
using ProcessingModule.Processing.SummedScan;

namespace ProcessingModule.ViewModels.SummedScan
{
    public class RaiseToPowerViewModel : INotifyPropertyChanged
    {
        private RaiseToPower _processing = new RaiseToPower();

        public double Power
        {
            get => _processing.Power;
            set
            {
                _processing.Power = Math.Round(value, 2);
                OnPropertyChanged(nameof(Power));
                EventAggregator.Instance.Invoke(this, new SumProcessingValuesChangedEventArgs());
            }
        }


        public RaiseToPowerViewModel()
        {
            EventAggregator.Instance.SumProcessingListChanged += OnProcessingListChanged;
        }


        private void OnProcessingListChanged(object obj, CmpCurvesSummation.Core.SumProcessingListChangedEventArgs e)
        {
            if (e.Processing.GetType() != typeof(RaiseToPower))
                return;
            _processing = (RaiseToPower)(e.Enabled == true ? e.Processing : null);
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