using System.Collections.ObjectModel;

namespace ProcessingModule.ViewModels
{
    public class ProcessingViewModel
    {
        //        public ObservableCollection<IRawDataProcessing> AvailableProcessings =
        //            new ObservableCollection<IRawDataProcessing>()
        //            {
        //                new ZeroAmplitudeCorrection()
        //            };
        //
        //        public ObservableCollection<IRawDataProcessing> CurrentProcessings = new ObservableCollection<IRawDataProcessing>();

        public ObservableCollection<string> AvailableProcessings =
            new ObservableCollection<string>()
            {
                "zero", "fvgikft"
            };

        public ObservableCollection<string> CurrentProcessings = new ObservableCollection<string>();

    }
}