using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CmpCurvesSummation.Core;
using CmpScanModule.Annotations;

namespace AppWithSimpleTestScan.ViewModels
{
    public delegate void AutoSummationCheckHander(object obj, AutoSummationCheckEventArgs e);
    public delegate void SummationHander(object obj, SummationClickEventArgs e);
    public delegate void PaletteChangedHander(object obj, PaletteChangedEventArgs e);



    public class OptionsViewModel : INotifyPropertyChanged
    {
        public const string Jet = "Jet";
        public const string Gray = "Gray";
        public const string BW = "B&W";

        public event AutoSummationCheckHander AutoSumCheckEvent;
        public event SummationHander SummationClick;
        public event PaletteChangedHander PaletteChanged;

        private bool _manualSummation;
        public bool ManualSummationPossible
        {
            get => _manualSummation;
            set
            {
                _manualSummation = value;
                OnPropertyChanged(nameof(ManualSummationPossible));
            }
        }
        private bool _cmpScanLoaded;

        private double _stepTime = CmpCurvesSummation.Core.CmpScan.DefaultStepTime;
        public double StepTime
        {
            get => _stepTime;
            set
            {
                _stepTime = value;
                if (CmpScan != null)
                    CmpScan.StepTime = value;
                OnPropertyChanged(nameof(StepTime));
            }
        }

        public ObservableCollection<double> StepsTime { get; set; } = new ObservableCollection<double>();

        private double _stepDistance = CmpCurvesSummation.Core.CmpScan.DefaultStepDistance;
        public double StepDistance
        {
            get => _stepDistance;
            set
            {
                _stepDistance = value;
                if (CmpScan != null)
                    CmpScan.StepDistance = value;
                OnPropertyChanged(nameof(StepDistance));
            }
        }
        public ObservableCollection<double> StepsDistance { get; set; } = new ObservableCollection<double>();

        private bool _autoSummation;
        public bool AutoSummation
        {
            get => _autoSummation;
            set
            {
                _autoSummation = value;
                ManualSummationPossible = !_autoSummation && _cmpScanLoaded;
                OnPropertyChanged(nameof(AutoSummation));
                AutoSumCheckEvent?.Invoke(this, new AutoSummationCheckEventArgs(value));
            }
        }

        public ObservableCollection<string> Palettes { get; set; } = new ObservableCollection<string>();

        private string _palette = Jet;
        public string Palette
        {
            get => _palette;
            set
            {
                _palette = value;
                OnPropertyChanged(nameof(Palette));
                PaletteType palette;
                switch (_palette)
                {
                    case Gray:
                        palette = PaletteType.Gray;
                        break;
                    case BW:
                        palette = PaletteType.BW;
                        break;
                    default:
                        palette = PaletteType.Jet;
                        break;
                }
                PaletteChanged?.Invoke(this, new PaletteChangedEventArgs(palette));
            }
        }

        public ICmpScan CmpScan { get; private set; }
        

        public OptionsViewModel()
        {
            InitStepsTime();
            InitStepsDistance();
            InitPalettes();
        }


        public void LaunchSummation()
        {
            SummationClick?.Invoke(this, new SummationClickEventArgs());
        }


        private void InitStepsDistance()
        {
            StepsDistance.Add(0.05);
            StepsDistance.Add(0.1);
            StepsDistance.Add(0.2);
            StepsDistance.Add(0.3);
            StepsDistance.Add(0.4);
            StepsDistance.Add(0.5);
            StepsDistance.Add(1);
        }

        private void InitStepsTime()
        {
            StepsTime.Add(0.5);
            StepsTime.Add(1);
            StepsTime.Add(2);
            StepsTime.Add(4);
        }

        private void InitPalettes()
        {
            Palettes.Add(Jet);
            Palettes.Add(Gray);
            Palettes.Add(BW);
        }


        public void OnFileLoaded(object sender, FileLoadedEventArgs e)
        {
            CmpScan = e.CmpScan;

            CmpScan.StepTime = _stepTime;
            CmpScan.StepDistance = _stepDistance;
        }

        public void OnRawCmpDataProcessed(object obj, RawCmpProcessedEventArgs args)
        {
            _cmpScanLoaded = true;
            ManualSummationPossible = !_autoSummation && _cmpScanLoaded;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}