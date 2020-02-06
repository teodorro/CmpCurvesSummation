﻿using System.ComponentModel;
using System.Runtime.CompilerServices;
using ProcessingModule.Annotations;
using ProcessingModule.Processing;

namespace ProcessingModule.ViewModels
{
    public class RemoveRightAscansViewModel : INotifyPropertyChanged
    {
        public event ProcessingListChangedHandler ProcessingListChanged;

        private RemoveRightAscans _processing;

        public int NumberOfAscans
        {
            get => _processing.NumberOfAscans;
            set
            {
                _processing.NumberOfAscans = value;
                OnPropertyChanged(nameof(NumberOfAscans));
                ProcessingListChanged(this, new ProcessingListChangedEventArgs() { Enabled = true, Processing = _processing });
            }
        }

        public int MaximumNumberOfAscans
        {
            get => _processing.MaximumNumberOfAscans;
            set
            {
                _processing.MaximumNumberOfAscans = value;
                OnPropertyChanged(nameof(MaximumNumberOfAscans));
                ProcessingListChanged(this, new ProcessingListChangedEventArgs() { Enabled = true, Processing = _processing });
            }
        }


        public RemoveRightAscansViewModel(RemoveRightAscans processing)
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