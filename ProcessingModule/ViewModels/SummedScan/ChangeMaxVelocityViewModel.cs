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
        public event SumProcessingListChangedHandler ProcessingListChanged;

        private ChangeMaxVelocity _processing;

        public double MaxVelocity
        {
            get => _processing.MaxVelocity;
            set
            {
                _processing.MaxVelocity = value;
                OnPropertyChanged(nameof(MaxVelocity));
                OnPropertyChanged(nameof(MaxVelocityCm));
                ProcessingListChanged(this, new SumProcessingListChangedEventArgs() { Enabled = true, Processing = _processing });
            }
        }
        public double MaxVelocityCm => Math.Round(MaxVelocity * 100, 2);

        public double PlotMaxVelocity => CmpMath.PlotMaxVelocity;
        public double PlotMinVelocity => CmpMath.PlotMinVelocity;


        public ChangeMaxVelocityViewModel(ChangeMaxVelocity processing)
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