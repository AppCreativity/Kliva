namespace Kliva.Models
{
    public class StatisticsGroup
    {
        public string Name { get; set; }
        public int Sort { get; set; }
    }

    public class StatisticsDetail
    {
        public StatisticsGroup Group { get; set; }
        public int Sort { get; set; }
        public string DisplayValue { get; set; }
        public string DisplayDescription { get; set; }
    }
}
