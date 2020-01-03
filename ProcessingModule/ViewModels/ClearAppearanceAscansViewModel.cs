using System.Collections.ObjectModel;

namespace ProcessingModule.ViewModels
{
    public class ClearAppearanceAscansViewModel
    {
        public int NumberOfAppearanceAscans { get; set; } = 0;

        public ObservableCollection<int> Numbers { get; set; } = new ObservableCollection<int>();

        public ClearAppearanceAscansViewModel()
        {
            for (int i = 0; i < 20; i++)
                Numbers.Add(i);
        }
    }
}