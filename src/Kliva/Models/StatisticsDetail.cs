using System.Collections.Generic;

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
}
