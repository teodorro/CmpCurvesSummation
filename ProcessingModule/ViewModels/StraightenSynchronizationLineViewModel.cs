namespace ProcessingModule.ViewModels
{
    public class StraightenSynchronizationLineViewModel
    {
        public StraightenSynchronizationLineViewModel(double minAmplitudeToCheck)
        {
            MinAmplitudeToCheck = minAmplitudeToCheck;
        }

        public double MinAmplitudeToCheck { get; set; } = 1;
    }
}