using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using CmpCurvesSummation.Core;
using ProcessingModule.Annotations;
using ProcessingModule.Processing.SummedScan;

namespace ProcessingModule.ViewModels
{
    public class SummedScanProcessingViewModel : INotifyPropertyChanged
    {
        private ISummedScanVT _sumScan;

        public ISummedScanProcessor Processor { get; }


        public SummedScanProcessingViewModel()
        {
            Processor = DiContainer.Instance.Container.GetInstance<ISummedScanProcessor>();
            EventAggregator.Instance.SummationFinished += OnSummationFinished;
            EventAggregator.Instance.SumProcessingListChanged += OnProcessingListChanged;
            EventAggregator.Instance.SumProcessingValuesChanged += OnProcessingValuesChanged;

        }


        private bool _changeMaxVelocityEnabled;
        public bool ChangeMaxVelocityEnabled
        {
            get => _changeMaxVelocityEnabled;
            set
            {
                _changeMaxVelocityEnabled = value;
                OnPropertyChanged(nameof(ChangeMaxVelocityEnabled));
                OnPropertyChanged(nameof(ChangeMaxVelocityEnabledVisible));
                EventAggregator.Instance.Invoke(this,
                    new SumProcessingListChangedEventArgs() { Enabled = value, Processing = Processor.OperationsAvailable.OfType<ChangeMaxVelocity>().FirstOrDefault() });
            }
        }
        public Visibility ChangeMaxVelocityEnabledVisible => ChangeMaxVelocityEnabled ? Visibility.Visible : Visibility.Collapsed;

        private bool _raiseToPowerEnabled;
        public bool RaiseToPowerEnabled
        {
            get => _raiseToPowerEnabled;
            set
            {
                _raiseToPowerEnabled = value;
                OnPropertyChanged(nameof(RaiseToPowerEnabled));
                OnPropertyChanged(nameof(RaiseToPowerEnabledVisible));
                EventAggregator.Instance.Invoke(this,
                    new SumProcessingListChangedEventArgs() { Enabled = value, Processing = Processor.OperationsAvailable.OfType<RaiseToPower>().FirstOrDefault() });
            }
        }
        public Visibility RaiseToPowerEnabledVisible => RaiseToPowerEnabled ? Visibility.Visible : Visibility.Collapsed;

        private bool _hideWeakValuesEnabled;
        public bool HideWeakValuesEnabled
        {
            get => _hideWeakValuesEnabled;
            set
            {
                _hideWeakValuesEnabled = value;
                OnPropertyChanged(nameof(HideWeakValuesEnabled));
                OnPropertyChanged(nameof(HideWeakValuesEnabledVisible));
                EventAggregator.Instance.Invoke(this,
                    new SumProcessingListChangedEventArgs() { Enabled = value, Processing = Processor.OperationsAvailable.OfType<HideWeakValues>().FirstOrDefault() });
            }
        }
        public Visibility HideWeakValuesEnabledVisible => HideWeakValuesEnabled ? Visibility.Visible : Visibility.Collapsed;

        private bool _absolutizeEnabled;
        public bool AbsolutizeEnabled
        {
            get => _absolutizeEnabled;
            set
            {
                _absolutizeEnabled = value;
                OnPropertyChanged(nameof(AbsolutizeEnabled));
                EventAggregator.Instance.Invoke(this,
                    new SumProcessingListChangedEventArgs()
                    {
                        Enabled = value,
                        Processing = Processor.OperationsAvailable.OfType<Absolutize>().FirstOrDefault()
                    });
                EventAggregator.Instance.Invoke(this, new SumProcessingValuesChangedEventArgs());
            }
        }


        private void OnSummationFinished(object o, SummationFinishedEventArgs e)
        {
            _sumScan = e.SummedScan;
        }

        private void OnProcessingValuesChanged(object obj, SumProcessingValuesChangedEventArgs e)
        {
            Processor.Process(_sumScan);
            if (_sumScan != null)
                EventAggregator.Instance.Invoke(this, new SumDataProcessedEventArgs(_sumScan));
        }

        private void OnProcessingListChanged(object obj, SumProcessingListChangedEventArgs e)
        {
            UpdateProcessingList(e);
        }

        private void UpdateProcessingList(SumProcessingListChangedEventArgs e)
        {
            var processing = e.Processing as ISumScanProcessing;
            if (e.Enabled == true && !Processor.OperationsToProcess.Contains(processing))
                Processor.OperationsToProcess.Add(processing);
            else if (e.Enabled == false && Processor.OperationsToProcess.Contains(processing))
                Processor.OperationsToProcess.Remove(processing);
            Processor.OperationsToProcess.Sort(new SumScanProcessingComparer());
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }



    public class SumScanProcessingComparer : IComparer<ISumScanProcessing>
    {
        public int Compare(ISumScanProcessing x, ISumScanProcessing y) => x.OrderIndex - y.OrderIndex;
    }
}