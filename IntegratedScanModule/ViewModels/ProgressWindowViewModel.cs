using System.ComponentModel;
using System.Runtime.CompilerServices;
using CmpCurvesSummation.Core;
using LayersInfoModule.Annotations;

namespace SummedScanModule.ViewModels
{
    public class ProgressWindowViewModel : INotifyPropertyChanged
    {
        private int _percent;

        public int Percent
        {
            get => _percent;
            set
            {
                _percent = value;
                OnPropertyChanged(nameof(Percent));
                OnPropertyChanged(nameof(LoadingText));
            }
        }

        public string LoadingText => $"{Percent}%";


        public ProgressWindowViewModel()
        {
            EventAggregator.Instance.SummationInProcess += OnSummationInProcess;
        }


        private void OnSummationInProcess(object o, SummationInProcessEventArgs args)
        {
            Percent = args.Percent;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}