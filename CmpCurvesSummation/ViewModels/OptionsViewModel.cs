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
            EventAggregator.Instance.CmpDataProcessed += OnCmpDataProcessed;

            InitPalettes();

            PointColors = GetColors();
            ItemsHodographColor = GetColors();

            SelectedPointColor = PointColors.First(x => x.Value == Colors.Black);
            SelectedItemHodographColor = ItemsHodographColor.First(x => x.Value == Colors.Black);
        }


        private bool _cmpScanLoaded;

        
        
        public ObservableCollection<string> Palettes { get; set; } = new ObservableCollection<string>();

        private string _palette = Jet;
        public string Palette
        {
            get => _palette;
            set
            {
                _palette = value;
                OnPropertyChanged(nameof(Palette));
                InvokeVisualOptionsChangedEvent();
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

        private KeyValuePair<string, Color> _selectedPointColor;
        public KeyValuePair<string, Color> SelectedPointColor
        {
            get => _selectedPointColor;
            set
            {
                _selectedPointColor = value;
                OnPropertyChanged(nameof(SelectedPointColor));
                InvokeVisualOptionsChangedEvent();
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
                OnPropertyChanged(nameof(SelectedItemHodographColor));
                InvokeVisualOptionsChangedEvent();
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
                InvokeVisualOptionsChangedEvent();
            }
        }

        private void InvokeVisualOptionsChangedEvent()
        {
            var palette = (PaletteType) new StringToPaletteConverter().Convert(_palette, null, null, null);
            EventAggregator.Instance.Invoke(this,
                new PlotVisualOptionsChangedEventArgs(palette, SelectedItemHodographColor.Value, SelectedPointColor.Value,
                    _interpolationEnabled,
                    _showHodographs, _showLayersProperties, _showAverageProperties));
        }

        private bool _showHodographs = true;
        public bool ShowHodographs
        {
            get => _showHodographs;
            set
            {
                _showHodographs = value;
                OnPropertyChanged(nameof(ShowHodographs));
                InvokeVisualOptionsChangedEvent();
            }
        }

        private bool _showLayersProperties = true;
        public bool ShowLayersProperties
        {
            get => _showLayersProperties;
            set
            {
                _showLayersProperties = value;
                OnPropertyChanged(nameof(ShowLayersProperties));
                InvokeVisualOptionsChangedEvent();
            }
        }

        private bool _showAverageProperties = true;
        public bool ShowAverageProperties
        {
            get => _showAverageProperties;
            set
            {
                _showAverageProperties = value;
                OnPropertyChanged(nameof(ShowAverageProperties));
                InvokeVisualOptionsChangedEvent();
            }
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

        private void OnCmpDataProcessed(object obj, CmpDataProcessedEventArgs args)
        {
            _cmpScanLoaded = true;
        }

        private IEnumerable<KeyValuePair<String, Color>> GetColors()
        {
            return typeof(Colors)
                .GetProperties()
                .Where(prop => typeof(Color).IsAssignableFrom(prop.PropertyType))
                .Select(prop => new KeyValuePair<String, Color>(prop.Name, (Color)prop.GetValue(null)));
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