using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CmpCurvesSummation.Core;
using CmpScanModule.Annotations;

namespace CmpCurvesSummation.ViewModels
{
    public class SurveyPropertiesViewModel : INotifyPropertyChanged
    {
        public ICmpScan CmpScan { get; private set; }


        public SurveyPropertiesViewModel()
        {
            EventAggregator.Instance.FileLoaded += OnFileLoaded;
            InitStepsTime();
            InitStepsDistance();
        }


        private double _stepTime = Core.CmpScan.DefaultStepTime;
        public double StepTime
        {
            get => _stepTime;
            set
            {
                var oldStepTime = _stepTime;
                _stepTime = value;
                if (CmpScan != null)
                    CmpScan.StepTime = value;
                OnPropertyChanged(nameof(StepTime));
                EventAggregator.Instance.Invoke(this,
                    new CmpScanParametersChangedEventArgs(CmpScan.StepDistance, oldStepTime, CmpScan.MinTime, CmpScan.StepDistance, CmpScan.StepTime, CmpScan.MinTime));
            }
        }

        public ObservableCollection<double> StepsTime { get; set; } = new ObservableCollection<double>();

        private double _stepDistance = Core.CmpScan.DefaultStepDistance;
        public double StepDistance
        {
            get => _stepDistance;
            set
            {
                var oldStepDistance = _stepDistance;
                _stepDistance = value;
                if (CmpScan != null)
                    CmpScan.StepDistance = value;
                OnPropertyChanged(nameof(StepDistance));
                EventAggregator.Instance.Invoke(this,
                    new CmpScanParametersChangedEventArgs(oldStepDistance, CmpScan.StepTime, CmpScan.MinTime, CmpScan.StepDistance, CmpScan.StepTime, CmpScan.MinTime));
            }
        }
        public ObservableCollection<double> StepsDistance { get; set; } = new ObservableCollection<double>();



        private void InitStepsDistance()
        {
            StepsDistance.Add(0.05);
            StepsDistance.Add(0.1);
            StepsDistance.Add(0.2);
            StepsDistance.Add(0.3);
            StepsDistance.Add(0.4);
            StepsDistance.Add(0.5);
            StepsDistance.Add(1);
        }

        private void InitStepsTime()
        {
            StepsTime.Add(0.5);
            StepsTime.Add(1);
            StepsTime.Add(2);
            StepsTime.Add(4);
        }

        private void OnFileLoaded(object sender, FileLoadedEventArgs e)
        {
            CmpScan = e.CmpScan;
            CmpScan.StepTime = _stepTime;
            CmpScan.StepDistance = _stepDistance;
        }

        
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}