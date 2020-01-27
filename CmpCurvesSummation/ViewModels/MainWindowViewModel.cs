using System.ComponentModel;
using System.Runtime.CompilerServices;
using CmpScanModule.Annotations;

namespace CmpCurvesSummation.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public const string TitleFixedPart = "Анализ годографов";
        private string _filename;

        public string Title
        {
            get => string.IsNullOrEmpty(_filename) 
                ? TitleFixedPart 
                : TitleFixedPart + " :: " + _filename;
            set
            {
                _filename = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}