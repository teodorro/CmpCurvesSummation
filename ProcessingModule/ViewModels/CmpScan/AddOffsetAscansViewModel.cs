﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CmpCurvesSummation.Core;
using ProcessingModule.Annotations;
using ProcessingModule.Processing.CmpScan;

namespace ProcessingModule.ViewModels.CmpScan
{
    public class AddOffsetAscansViewModel : INotifyPropertyChanged
    {
        private AddOffsetAscans _processing = new AddOffsetAscans();

        public int NumberOfOffsetAscans
        {
            get => _processing.NumberOfAscans;
            set
            {
                if (_processing == null)
                    return;
                _processing.NumberOfAscans = value;
                OnPropertyChanged(nameof(NumberOfOffsetAscans));
                EventAggregator.Instance.Invoke(this, new CmpProcessingValuesChangedEventArgs());
            }
        }


        public AddOffsetAscansViewModel()
        {
            EventAggregator.Instance.CmpProcessingListChanged += OnProcessingListChanged;
            EventAggregator.Instance.FileLoaded += OnFileLoaded;
        }


        private void OnFileLoaded(object obj, FileLoadedEventArgs e)
        {
            NumberOfOffsetAscans = AddOffsetAscans.DefaultValue;
        }
        
        private void OnProcessingListChanged(object obj, CmpProcessingListChangedEventArgs e)
        {
            if (e.Processing.GetType() != typeof(AddOffsetAscans))
                return;
            _processing = (AddOffsetAscans) (e.Enabled == true ? e.Processing : null);
            if (_processing != null)
                NumberOfOffsetAscans = _processing.NumberOfAscans;
//            EventAggregator.Instance.Invoke(this, new CmpProcessingValuesChangedEventArgs());

        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}