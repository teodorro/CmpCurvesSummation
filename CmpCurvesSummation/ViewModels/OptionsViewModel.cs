using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using CmpCurvesSummation.Core;
using CmpScanModule.Annotations;

namespace CmpCurvesSummation.ViewModels
{
    // TODO: 3 states
    // 1. nothing is loaded
    // 2. the process of summation
    // 3. file loaded, waiting for user actions

    public class OptionsViewModel : INotifyPropertyChanged
    {
        public const string Jet = "Jet";
        public const string Gray = "Gray";
        public const string BW = "B&W";
        
        public event SummationStartedHander SummationStarted;
        public event PaletteChangedHander PaletteChanged;
        public event StepDistanceChangedHandler StepDistanceChanged;
        public event StepTimeChangedHandler StepTimeChanged;

        public ICmpScan CmpScan { get; private set; }

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
                var oldStepTime = _stepTime;
                _stepTime = value;
                if (CmpScan != null)
                    CmpScan.StepTime = value;
                OnPropertyChanged(nameof(StepTime));
                StepTimeChanged?.Invoke(this, new StepTimeEventArgs(_stepTime, oldStepTime, CmpScan));
            }
        }

        public ObservableCollection<double> StepsTime { get; set; } = new ObservableCollection<double>();

        private double _stepDistance = CmpCurvesSummation.Core.CmpScan.DefaultStepDistance;
        public double StepDistance
        {
            get => _stepDistance;
            set
            {
                var oldStepDistance = _stepDistance;
                _stepDistance = value;
                if (CmpScan != null)
                    CmpScan.StepDistance = value;
                OnPropertyChanged(nameof(StepDistance));
                StepDistanceChanged?.Invoke(this, new StepDistanceEventArgs(_stepDistance, oldStepDistance, CmpScan));
            }
        }
        public ObservableCollection<double> StepsDistance { get; set; } = new ObservableCollection<double>();
        
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

        private bool _progressBarVisible = false;
        public Visibility ProgressBarVisible
        {
            get => _progressBarVisible ? Visibility.Visible : Visibility.Hidden;
            set
            {
                _progressBarVisible = value == Visibility.Visible;
                OnPropertyChanged(nameof(ProgressBarVisible));
            }
        }

        private int _progressValue;
        public int ProgressValue
        {
            get => _progressValue;
            set
            {
                _progressValue = value;
                OnPropertyChanged(nameof(ProgressValue));
            }
        }


        public OptionsViewModel()
        {
            InitStepsTime();
            InitStepsDistance();
            InitPalettes();
        }


        public void LaunchSummation()
        {
            ManualSummationPossible = false;
            ProgressBarVisible = Visibility.Visible;
            SummationStarted?.Invoke(this, new SummationStartedClickEventArgs());
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
            ManualSummationPossible = _cmpScanLoaded;
        }

        public void OnSummationFinished(object obj, SummationFinishedEventArgs e)
        {
            ManualSummationPossible = true;
            ProgressBarVisible = Visibility.Hidden;
            ProgressValue = 0;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void OnSummationInProcess(object obj, SummationInProcessEventArgs e)
        {
            ProgressValue = e.Percent;
        }
    }


    public class ColorToSolidBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new SolidColorBrush((Color)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }





}