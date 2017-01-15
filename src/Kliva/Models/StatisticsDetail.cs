using System.Collections.Generic;
using System.ComponentModel;

namespace Kliva.Models
{
    public enum StatisticGroupType
    {
        Current,
        PR
    }

    public class StatisticsGroup
    {
        public string Name { get; set; }
        public int Sort { get; set; }
        public StatisticGroupType Type { get; set; }

        public List<StatisticsDetail> Details { get; set; } = new List<StatisticsDetail>();
    }

    public class StatisticsDetail
    {
        public StatisticsGroup Group { get; set; }
        public int Sort { get; set; }
        public string DisplayValue { get; set; }
        public string DisplayDescription { get; set; }

        public string Icon { get; set; }
    }

    public class UserMeasurementUnitStatisticsDetail : StatisticsDetail, INotifyPropertyChanged
    {
        public UserMeasurementUnitMetric Metric { get; private set; }
        public UserMeasurementUnitStatisticsDetail(UserMeasurementUnitMetric metric)
        {
            Metric = metric;
            Metric.PropertyChanged += OnMetricPropertyChanged;
            base.DisplayValue = metric.FormattedValueWithUnit;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnMetricPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(UserMeasurementUnitMetric.FormattedValueWithUnit))
            {
                DisplayValue = Metric.FormattedValueWithUnit;
            }
        }

        public new string DisplayValue
        {
            get
            {
                return base.DisplayValue;
            }
            private set
            {
                if(DisplayValue != value)
                {
                    base.DisplayValue = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DisplayValue)));
                }
            }
        }

        public void Unload()
        {
            Metric.PropertyChanged -= OnMetricPropertyChanged;
        }
    }
}
