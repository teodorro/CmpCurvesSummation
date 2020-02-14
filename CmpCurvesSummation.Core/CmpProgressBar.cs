using System;

namespace CmpCurvesSummation.Core
{
    public class CmpProgressBar
    {
        private static Lazy<CmpProgressBar> _instance = new Lazy<CmpProgressBar>(() => new CmpProgressBar());
        public static CmpProgressBar Instance => _instance.Value;


        private int _progressValue;
        public int ProgressValue
        {
            get => _progressValue;
            set
            {
                _progressValue = value;
                EventAggregator.Instance.Invoke(this, new SummationInProcessEventArgs(value));
            }
        }
    }
}