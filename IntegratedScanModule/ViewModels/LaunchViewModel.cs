using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using CmpCurvesSummation.Core;
using LayersInfoModule.Annotations;

namespace SummedScanModule.ViewModels
{
    public class LaunchViewModel : INotifyPropertyChanged
    {
        public LaunchViewModel()
        {
            EventAggregator.Instance.SummationInProcess += OnSummationInProcess;
            EventAggregator.Instance.SumDataProcessed += OnSumProcessed;
            EventAggregator.Instance.CmpDataProcessed += OnCmpDataProcessed;
        }


        private bool _manualSummation;
        public bool ManualSummationPossible
        {
            get => _manualSummation;
            set
            {
                _manualSummation = value;
                OnPropertyChanged(nameof(ManualSummationPossible));
            }
        }

        private bool _progressBarVisible = false;
        public Visibility ProgressBarVisible
        {
            get => _progressBarVisible ? Visibility.Visible : Visibility.Hidden;
            set
            {
                _progressBarVisible = value == Visibility.Visible;
                OnPropertyChanged(nameof(ProgressBarVisible));
            }
        }

        private int _progressValue;
        public int ProgressValue
        {
            get => _progressValue;
            set
            {
                _progressValue = value;
                OnPropertyChanged(nameof(ProgressValue));
            }
        }

        public void LaunchSummation()
        {
            ProgressBarVisible = Visibility.Visible;
            EventAggregator.Instance.Invoke(this, new SummationStartedEventArgs());
        }

        private void OnSumProcessed(object obj, SumDataProcessedEventArgs e)
        {
            ProgressBarVisible = Visibility.Hidden;
            ProgressValue = 0;
        }

        private void OnSummationInProcess(object obj, SummationInProcessEventArgs e)
        {
            ProgressValue = e.Percent;
        }

        private void OnCmpDataProcessed(object obj, CmpDataProcessedEventArgs args)
        {
            ManualSummationPossible = true;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}