using System;
using System.Collections.ObjectModel;

namespace ProcessingModule.ViewModels
{
    public class ProcessingViewModel
    {
        public ObservableCollection<IRawDataProcessing> ChooseList { get; } = new ObservableCollection<IRawDataProcessing>();

        public ObservableCollection<IRawDataProcessing> ProcessingList { get; } = new ObservableCollection<IRawDataProcessing>();
    }
}