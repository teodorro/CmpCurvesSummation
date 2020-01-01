using System.Collections.ObjectModel;
using ProcessingModule.Processing;

namespace ProcessingModule.ViewModels
{
    public class ProcessingViewModel
    {
        public ObservableCollection<IRawDataProcessing> ChooseList { get; } = new ObservableCollection<IRawDataProcessing>() { new ZeroAmplitudeCorrection() };

        public ObservableCollection<IRawDataProcessing> ProcessingList { get; } = new ObservableCollection<IRawDataProcessing>();
    }
}