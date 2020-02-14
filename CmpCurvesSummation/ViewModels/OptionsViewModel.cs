using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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
        public const string Hot = "Hot";
        public const string Rainbow = "Rainbow";
        public const string HueDistinct = "HueDistinct";
        public const string Hue = "Hue";
        public const string BlackWhiteRed = "BlackWhiteRed";
        public const string BlueWhiteRed = "BlueWhiteRed";
        public const string Cool = "Cool";
        

        public OptionsViewModel()
        {
            EventAggregator.Instance.SummationInProcess += OnSummationInProcess;
            EventAggregator.Instance.FileLoaded += OnFileLoaded;
            EventAggregator.Instance.CmpDataProcessed += OnCmpDataProcessed;
            EventAggregator.Instance.SumDataProcessed += OnSumProcessed;

            InitStepsTime();
            InitStepsDistance();
            InitPalettes();

            PointColors = GetColors();
            ItemsHodographColor = GetColors();
        }


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

        private double _stepTime = Core.CmpScan.DefaultStepTime;
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
                EventAggregator.Instance.Invoke(this, 
                    new CmpScanParametersChangedEventArgs(CmpScan.StepDistance, oldStepTime, CmpScan.MinTime, CmpScan.StepDistance, CmpScan.StepTime, CmpScan.MinTime));
            }
        }

        public ObservableCollection<double> StepsTime { get; set; } = new ObservableCollection<double>();

        private double _stepDistance = Core.CmpScan.DefaultStepDistance;
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
                EventAggregator.Instance.Invoke(this,
                    new CmpScanParametersChangedEventArgs(oldStepDistance, CmpScan.StepTime, CmpScan.MinTime, CmpScan.StepDistance, CmpScan.StepTime, CmpScan.MinTime));
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
                var palette = (PaletteType)new StringToPaletteConverter().Convert(_palette, null, null, null);
                EventAggregator.Instance.Invoke(this, 
                    new PlotVisualOptionsChangedEventArgs(palette, SelectedItemHodographColor.Value, SelectedPointColor.Value, _interpolationEnabled));
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

        private IEnumerable<KeyValuePair<String, Color>> _pointColors;
        public IEnumerable<KeyValuePair<String, Color>> PointColors
        {
            get => _pointColors;
            set
            {
                _pointColors = value;
                OnPropertyChanged(nameof(PointColors));
            }
        }

        private KeyValuePair<String, Color> _selectedPointColor;
        public KeyValuePair<String, Color> SelectedPointColor
        {
            get => _selectedPointColor;
            set
            {
                _selectedPointColor = value;
                var palette = (PaletteType)new StringToPaletteConverter().Convert(_palette, null, null, null);
                EventAggregator.Instance.Invoke(this,
                    new PlotVisualOptionsChangedEventArgs(palette, SelectedItemHodographColor.Value, SelectedPointColor.Value, _interpolationEnabled));
                OnPropertyChanged(nameof(SelectedPointColor));
            }
        }
        
        private IEnumerable<KeyValuePair<String, Color>> _itemsHodographColor;
        public IEnumerable<KeyValuePair<String, Color>> ItemsHodographColor
        {
            get => _itemsHodographColor;
            set
            {
                _itemsHodographColor = value;
                OnPropertyChanged(nameof(ItemsHodographColor));
            }
        }

        private KeyValuePair<String, Color> _selectedItemItemHodographColor;
        public KeyValuePair<String, Color> SelectedItemHodographColor
        {
            get => _selectedItemItemHodographColor;
            set
            {
                _selectedItemItemHodographColor = value;
                var palette = (PaletteType)new StringToPaletteConverter().Convert(_palette, null, null, null);
                EventAggregator.Instance.Invoke(this,
                    new PlotVisualOptionsChangedEventArgs(palette, SelectedItemHodographColor.Value, SelectedPointColor.Value, _interpolationEnabled));
                OnPropertyChanged(nameof(SelectedItemHodographColor));
            }
        }

        private bool _interpolationEnabled = false;
        public bool InterpolationEnabled
        {
            get => _interpolationEnabled;
            set
            {
                _interpolationEnabled = value;
                OnPropertyChanged(nameof(InterpolationEnabled));
                var palette = (PaletteType)new StringToPaletteConverter().Convert(_palette, null, null, null);
                EventAggregator.Instance.Invoke(this,
                    new PlotVisualOptionsChangedEventArgs(palette, SelectedItemHodographColor.Value, SelectedPointColor.Value, _interpolationEnabled));
            }
        }


        public void LaunchSummation()
        {
            ManualSummationPossible = false;
            ProgressBarVisible = Visibility.Visible;
            EventAggregator.Instance.Invoke(this, new SummationStartedEventArgs());
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
            Palettes.Add(Rainbow);
            Palettes.Add(Hot);
            Palettes.Add(Cool);
            Palettes.Add(Hue);
            Palettes.Add(HueDistinct);
            Palettes.Add(BlackWhiteRed);
            Palettes.Add(BlueWhiteRed);
        }

        private void OnFileLoaded(object sender, FileLoadedEventArgs e)
        {
            CmpScan = e.CmpScan;
            CmpScan.StepTime = _stepTime;
            CmpScan.StepDistance = _stepDistance;
        }

        private void OnCmpDataProcessed(object obj, CmpDataProcessedEventArgs args)
        {
            _cmpScanLoaded = true;
            ManualSummationPossible = _cmpScanLoaded;
        }

        private void OnSumProcessed(object obj, SumDataProcessedEventArgs e)
        {
            ManualSummationPossible = true;
            ProgressBarVisible = Visibility.Hidden;
            ProgressValue = 0;
        }

        private IEnumerable<KeyValuePair<String, Color>> GetColors()
        {
            return typeof(Colors)
                .GetProperties()
                .Where(prop => typeof(Color).IsAssignableFrom(prop.PropertyType))
                .Select(prop => new KeyValuePair<String, Color>(prop.Name, (Color)prop.GetValue(null)));
        }

        private void OnSummationInProcess(object obj, SummationInProcessEventArgs e)
        {
            ProgressValue = e.Percent;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }



    public class ColorToSolidBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            => new SolidColorBrush((Color)value);

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            => throw new NotImplementedException();
    }



    public class StringToPaletteConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var paletteString = (string) value;
            PaletteType palette;
            switch (paletteString)
            {
                case "Gray":
                    palette = PaletteType.Gray;
                    break;
                case "Rainbow":
                    palette = PaletteType.Rainbow;
                    break;
                case "Hot":
                    palette = PaletteType.Hot;
                    break;
                case "Cool":
                    palette = PaletteType.Cool;
                    break;
                case "HueDistinct":
                    palette = PaletteType.HueDistinct;
                    break;
                case "Hue":
                    palette = PaletteType.Hue;
                    break;
                case "BlackWhiteRed":
                    palette = PaletteType.BlackWhiteRed;
                    break;
                case "BlueWhiteRed":
                    palette = PaletteType.BlueWhiteRed;
                    break;
                default:
                    palette = PaletteType.Jet;
                    break;
            }
            return palette;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var palette = (PaletteType)value;
            string paletteString;
            switch (palette)
            {
                case PaletteType.Gray:
                    paletteString = "Gray";
                    break;
                case PaletteType.Rainbow:
                    paletteString = "Rainbow";
                    break;
                case PaletteType.Hot:
                    paletteString = "Hot";
                    break;
                case PaletteType.Cool:
                    paletteString = "Cool";
                    break;
                case PaletteType.HueDistinct:
                    paletteString = "HueDistinct";
                    break;
                case PaletteType.Hue:
                    paletteString = "Hue";
                    break;
                case PaletteType.BlackWhiteRed:
                    paletteString = "BlackWhiteRed";
                    break;
                case PaletteType.BlueWhiteRed:
                    paletteString = "BlueWhiteRed";
                    break;
                default:
                    paletteString = "Jet";
                    break;
            }
            return paletteString;
        }
    }
}