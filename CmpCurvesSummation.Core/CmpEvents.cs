using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace CmpCurvesSummation.Core
{
//    public enum PaletteType
//    {
//        Jet,
//        Gray,
//        Rainbow,
//        Hot,
//        HueDistinct,
//        Hue,
//        BlackWhiteRed,
//        BlueWhiteRed,
//        Cool
//    }
//
//
//    public delegate void PaletteChangedHander(object obj, PaletteChangedEventArgs e);
//    public delegate void StepDistanceChangedHandler(object obj, StepDistanceEventArgs e);
//    public delegate void StepTimeChangedHandler(object obj, StepTimeEventArgs e);
//    public delegate void AutoCorrectionCheckHander(object obj, AutoCorrectionCheckEventArgs e);
//    public delegate void HodographColorChangedHandler(object obj, HodographColorChangedEventArgs e);
//    public delegate void PointColorChangedHandler(object obj, PointColorChangedEventArgs e);
//    public delegate void RefreshLayersHandler(object obj, RefreshLayersEventArgs e);
//    public delegate void TimeOffsetChangedHandler(object obj, TimeOffsetChangedEventArgs e);
//    public delegate void InterpolationChangedHandler(object obj, InterpolationChangedEventArgs e);
//    public delegate void AlphaChangedHandler(object obj, AlphaChangedEventArgs e);
//    public delegate void HalfWaveSizeChangedHandler(object obj, HalfWaveSizeChangedEventArgs e);
//
//
//
//    
//    
//
//    public class SummedOverHodographEventArgs : EventArgs
//    {
//        public ISummedScanVH SummedScan { get; }
//        public SummedOverHodographEventArgs(ISummedScanVH summedScan)
//        {
//            SummedScan = summedScan;
//        }
//    }
//
//
//    public class HodographDrawVHClickEventArgs : EventArgs
//    {
//        public HodographDrawVHClickEventArgs(double velocity, double height)
//        {
//            Velocity = velocity;
//            Height = height;
//        }
//
//        public double Velocity { get; }
//        public double Height { get; }
//    }
//
//
//    public class HodographDrawVTClickEventArgs : EventArgs
//    {
//        public HodographDrawVTClickEventArgs(double velocity, double time)
//        {
//            Velocity = velocity;
//            Time = time;
//        }
//
//        public double Velocity { get; }
//        public double Time { get;  }
//    }
//
//
//    public class DeleteLayerEventArgs : EventArgs
//    {
//        public DeleteLayerEventArgs(double velocity, double time)
//        {
//            Velocity = velocity;
//            Time = time;
//        }
//
//        public double Velocity { get; }
//        public double Time { get; }
//    }
//
//
//    public class AutoSummationCheckEventArgs : EventArgs
//    {
//        public AutoSummationCheckEventArgs(bool auto)
//        {
//            Auto = auto;
//        }
//
//        public bool Auto { get; }
//    }
//
//
//    
//
//
//    public class PaletteChangedEventArgs : EventArgs
//    {
//        public PaletteChangedEventArgs(PaletteType palette)
//        {
//            Palette = palette;
//        }
//
//        public PaletteType Palette { get; }
//    }
//
//
//    public class StepDistanceEventArgs : EventArgs
//    {
//        public StepDistanceEventArgs(double newStepDistance, double oldStepDistance, ICmpScan cmpScan)
//        {
//            NewStepDistance = newStepDistance;
//            OldStepDistance = oldStepDistance;
//            CmpScan = cmpScan;
//        }
//
//        public double NewStepDistance { get; }
//        public double OldStepDistance { get; }
//        public ICmpScan CmpScan { get; }
//    }
//
//
//    public class StepTimeEventArgs : EventArgs
//    {
//        public StepTimeEventArgs(double newStepTime, double oldStepTime, ICmpScan cmpScan)
//        {
//            NewStepTime = newStepTime;
//            OldStepTime = oldStepTime;
//            CmpScan = cmpScan;
//        }
//
//        public double NewStepTime { get; }
//        public double OldStepTime { get; }
//        public ICmpScan CmpScan { get; }
//    }
//
//
//    public class AutoCorrectionCheckEventArgs : EventArgs
//    {
//        public AutoCorrectionCheckEventArgs(bool auto)
//        {
//            Auto = auto;
//        }
//
//        public bool Auto { get; }
//    }
//
//
//    public class HodographColorChangedEventArgs : EventArgs
//    {
//        public Color NewColor { get; }
//
//        public HodographColorChangedEventArgs(Color newColor)
//        {
//            NewColor = newColor;
//        }
//    }
//
//    public class PointColorChangedEventArgs : EventArgs
//    {
//        public Color NewColor { get; }
//
//        public PointColorChangedEventArgs(Color newColor)
//        {
//            NewColor = newColor;
//        }
//    }
//
//    public class RefreshLayersEventArgs : EventArgs
//    {
//        public RefreshLayersEventArgs(IEnumerable<Tuple<double, double>> layers)
//        {
//            Layers = layers;
//        }
//
//        public IEnumerable<Tuple<double, double>> Layers { get; }
//    }
//
//    public class TimeOffsetChangedEventArgs : EventArgs
//    {
//        public TimeOffsetChangedEventArgs(double timeOffset, ICmpScan cmpScan)
//        {
//            TimeOffset = timeOffset;
//            CmpScan = cmpScan;
//        }
//
//        public double TimeOffset { get; }
//        public ICmpScan CmpScan { get; }
//    }
//
//    public class InterpolationChangedEventArgs : EventArgs
//    {
//        public bool Interpolation{ get; }
//
//        public InterpolationChangedEventArgs(bool interpolation)
//        {
//            Interpolation = interpolation;
//        }
//    }
//
//    public class AlphaChangedEventArgs : EventArgs
//    {
//        public byte Alpha { get; }
//
//        public AlphaChangedEventArgs(byte alpha)
//        {
//            Alpha = alpha;
//        }
//    }
//    
//    public class HalfWaveSizeChangedEventArgs : EventArgs
//    {
//        public int HalfWaveSize { get; }
//
//        public HalfWaveSizeChangedEventArgs(int halfWaveSize)
//        {
//            HalfWaveSize = halfWaveSize;
//        }
//    }














}