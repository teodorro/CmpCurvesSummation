using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CmpCurvesSummation.Core;
using ProcessingModule.Annotations;
using ProcessingModule.Processing.SummedScan;

namespace ProcessingModule.ViewModels.SummedScan
{
    public class ChangeMaxVelocityViewModel : INotifyPropertyChanged
    {
        private ChangeMaxVelocity _processing = new ChangeMaxVelocity();

        public double MaxVelocity
        {
            get => _processing.MaxVelocity;
            set
            {
                _processing.MaxVelocity = value;
                OnPropertyChanged(nameof(MaxVelocity));
                OnPropertyChanged(nameof(MaxVelocityCm));
                EventAggregator.Instance.Invoke(this, new SumProcessingValuesChangedEventArgs());
            }
        }
        public double MaxVelocityCm => Math.Round(MaxVelocity * 100, 2);

        public double PlotMaxVelocity => CmpMath.PlotMaxVelocity;
        public double PlotMinVelocity => CmpMath.PlotMinVelocity;


        public ChangeMaxVelocityViewModel()
        {
            EventAggregator.Instance.SumProcessingListChanged += OnProcessingListChanged;
        }


        private void OnProcessingListChanged(object obj, CmpCurvesSummation.Core.SumProcessingListChangedEventArgs e)
        {
            if (e.Processing.GetType() != typeof(ChangeMaxVelocity))
                return;
            _processing = (ChangeMaxVelocity)(e.Enabled == true ? e.Processing : null);
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