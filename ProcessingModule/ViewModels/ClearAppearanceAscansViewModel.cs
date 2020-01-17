using System.Collections.ObjectModel;

namespace ProcessingModule.ViewModels
{
    public class ClearAppearanceAscansViewModel
    {
        private const int _maxAppearanceAscansCount = 20;

        public int NumberOfAppearanceAscans { get; set; } = 5;

        public ObservableCollection<int> Numbers { get; set; } = new ObservableCollection<int>();


        public ClearAppearanceAscansViewModel(int numberOfAppearanceAscans)
        {
            NumberOfAppearanceAscans = numberOfAppearanceAscans;
            for (int i = 0; i < _maxAppearanceAscansCount; i++)
                Numbers.Add(i);
        }
    }
}