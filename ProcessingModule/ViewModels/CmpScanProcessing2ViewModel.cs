using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using CmpCurvesSummation.Core;
using ProcessingModule.Annotations;
using ProcessingModule.Processing.CmpScan;

namespace ProcessingModule.ViewModels
{
    public class CmpScanProcessing2ViewModel : INotifyPropertyChanged
    {
        private ICmpScan _cmpScan;

        public ICmpScanProcessor Processor { get; }


        public CmpScanProcessing2ViewModel()
        {
            Processor = DiContainer.Instance.Container.GetInstance<ICmpScanProcessor>();
            EventAggregator.Instance.FileLoaded += OnFileLoaded;
            EventAggregator.Instance.CmpProcessingListChanged += OnProcessingListChanged;
            EventAggregator.Instance.CmpProcessingValuesChanged += OnProcessingValuesChanged;
        }


        private bool _addOffsetAscansEnabled;
        public bool AddOffsetAscansEnabled
        {
            get => _addOffsetAscansEnabled;
            set
            {
                _addOffsetAscansEnabled = value;
                OnPropertyChanged(nameof(AddOffsetAscansEnabled));
                OnPropertyChanged(nameof(AddOffsetAscansEnabledVisible));
                EventAggregator.Instance.Invoke(this,
                    new CmpProcessingListChangedEventArgs() { Enabled = value, Processing = Processor.OperationsAvailable.OfType<AddOffsetAscans>().FirstOrDefault() });
            }
        }
        public Visibility AddOffsetAscansEnabledVisible => _addOffsetAscansEnabled ? Visibility.Visible : Visibility.Collapsed;

        private bool _clearOffsetAscansEnabled;
        public bool ClearOffsetAscansEnabled
        {
            get => _clearOffsetAscansEnabled;
            set
            {
                _clearOffsetAscansEnabled = value;
                OnPropertyChanged(nameof(ClearOffsetAscansEnabled));
                OnPropertyChanged(nameof(ClearOffsetAscansEnabledVisible));
                EventAggregator.Instance.Invoke(this,
                    new CmpProcessingListChangedEventArgs() { Enabled = value, Processing = Processor.OperationsAvailable.OfType<ClearOffsetAscans>().FirstOrDefault() });
            }
        }
        public Visibility ClearOffsetAscansEnabledVisible => _clearOffsetAscansEnabled ? Visibility.Visible : Visibility.Collapsed;

        private bool _removeLeftAscansEnabled = false;
        public bool RemoveLeftAscansEnabled
        {
            get => _removeLeftAscansEnabled;
            set
            {
                _removeLeftAscansEnabled = value;
                OnPropertyChanged(nameof(RemoveLeftAscansEnabled));
                OnPropertyChanged(nameof(RemoveLeftAscansEnabledVisible));
                EventAggregator.Instance.Invoke(this,
                    new CmpProcessingListChangedEventArgs() { Enabled = value, Processing = Processor.OperationsAvailable.OfType<RemoveLeftAscans>().FirstOrDefault() });
            }
        }
        public Visibility RemoveLeftAscansEnabledVisible => _removeLeftAscansEnabled ? Visibility.Visible : Visibility.Collapsed;

        private bool _removeRightAscansEnabled;
        public bool RemoveRightAscansEnabled
        {
            get => _removeRightAscansEnabled;
            set
            {
                _removeRightAscansEnabled = value;
                OnPropertyChanged(nameof(RemoveRightAscansEnabled));
                OnPropertyChanged(nameof(RemoveRightAscansEnabledVisible));
                EventAggregator.Instance.Invoke(this,
                    new CmpProcessingListChangedEventArgs() { Enabled = value, Processing = Processor.OperationsAvailable.OfType<RemoveRightAscans>().FirstOrDefault() });
            }
        }
        public Visibility RemoveRightAscansEnabledVisible => _removeRightAscansEnabled ? Visibility.Visible : Visibility.Collapsed;

        private bool _straightenSynchronizationLineEnabled;
        public bool StraightenSynchronizationLineEnabled
        {
            get => _straightenSynchronizationLineEnabled;
            set
            {
                _straightenSynchronizationLineEnabled = value;
                OnPropertyChanged(nameof(StraightenSynchronizationLineEnabled));
                OnPropertyChanged(nameof(StraightenSynchronizationLineEnabledVisible));
                EventAggregator.Instance.Invoke(this,
                    new CmpProcessingListChangedEventArgs() { Enabled = value, Processing = Processor.OperationsAvailable.OfType<StraightenSynchronizationLine>().FirstOrDefault() });
            }
        }
        public Visibility StraightenSynchronizationLineEnabledVisible => _straightenSynchronizationLineEnabled ? Visibility.Visible : Visibility.Collapsed;

        private bool _logarithmProcessingEnabled;
        public bool LogarithmProcessingEnabled
        {
            get => _logarithmProcessingEnabled;
            set
            {
                _logarithmProcessingEnabled = value;
                OnPropertyChanged(nameof(LogarithmProcessingEnabled));
                EventAggregator.Instance.Invoke(this,
                    new CmpProcessingListChangedEventArgs()
                    {
                        Enabled = value,
                        Processing = Processor.OperationsAvailable.OfType<LogarithmProcessing>().FirstOrDefault()
                    });
                EventAggregator.Instance.Invoke(this, new CmpDataProcessedEventArgs(_cmpScan));
//                Task.Run(() => EventAggregator.Instance.Invoke(this,
//                    new CmpProcessingListChangedEventArgs() { Enabled = value, Processing = Processor.OperationsAvailable.OfType<LogarithmProcessing>().FirstOrDefault() }))
//                    .ContinueWith(_ => EventAggregator.Instance.Invoke(this, new CmpDataProcessedEventArgs(_cmpScan)));
            }
        }

        private bool _zeroAmplitudeCorrectionEnabled;
        public bool ZeroAmplitudeCorrectionEnabled
        {
            get => _zeroAmplitudeCorrectionEnabled;
            set
            {
                _zeroAmplitudeCorrectionEnabled = value;
                OnPropertyChanged(nameof(ZeroAmplitudeCorrectionEnabled));
                EventAggregator.Instance.Invoke(this,
                    new CmpProcessingListChangedEventArgs()
                    {
                        Enabled = value,
                        Processing = Processor.OperationsAvailable.OfType<ZeroAmplitudeCorrection>().FirstOrDefault()
                    });
                EventAggregator.Instance.Invoke(this, new CmpProcessingValuesChangedEventArgs());
//                Task.Run(() => EventAggregator.Instance.Invoke(this,
//                    new CmpProcessingListChangedEventArgs() { Enabled = value, Processing = Processor.OperationsAvailable.OfType<ZeroAmplitudeCorrection>().FirstOrDefault() }))
//                    .ContinueWith(_ => EventAggregator.Instance.Invoke(this, new CmpDataProcessedEventArgs(_cmpScan)));
            }
        }

        private void OnFileLoaded(object sender, FileLoadedEventArgs e)
        {
            _cmpScan = e.CmpScan;
            Processor.RefreshOperations(_cmpScan);
            Processor.Process(_cmpScan);
            SetDefaultValues();
//            EventAggregator.Instance.Invoke(this, new CmpDataProcessedEventArgs(_cmpScan));
        }

        private void SetDefaultValues()
        {
            ZeroAmplitudeCorrectionEnabled = true;
        }

        private void OnProcessingValuesChanged(object obj, CmpProcessingValuesChangedEventArgs e)
        {
            Processor.Process(_cmpScan);
            if (_cmpScan != null)
                EventAggregator.Instance.Invoke(this, new CmpDataProcessedEventArgs(_cmpScan));
        }

        private void OnProcessingListChanged(object sender, CmpProcessingListChangedEventArgs e)
        {
            UpdateProcessingList(e);
        }

        private void UpdateProcessingList(CmpProcessingListChangedEventArgs e)
        {
            var processing = e.Processing as ICmpScanProcessing;
            if (e.Enabled == true && !Processor.OperationsToProcess.Contains(processing))
                Processor.OperationsToProcess.Add(processing);
            else if (e.Enabled == false && Processor.OperationsToProcess.Contains(processing))
                Processor.OperationsToProcess.Remove(processing);
            Processor.OperationsToProcess.Sort(new CmpScanProcessingComparer());
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }



    public class CmpScanProcessingComparer : IComparer<ICmpScanProcessing>
    {
        public int Compare(ICmpScanProcessing x, ICmpScanProcessing y) => x.OrderIndex - y.OrderIndex;
    }




}