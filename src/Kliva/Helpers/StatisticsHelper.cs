using Kliva.Models;
using System.Collections.Generic;

namespace Kliva.Helpers
{
    public static class StatisticsHelper
    {
        private static StatisticsGroup CreateGroup(string name, int sort)
        {
            return new StatisticsGroup() { Name = name, Sort = sort };
        }

        public static void CreateDetailForGroup(StatisticsGroup group, int sort, string icon, 
            string displayName, string displayValue)
        {
            var detail = new StatisticsDetail()
            {
                Sort = sort,
                Icon = icon,
                DisplayDescription = displayName,
                DisplayValue = displayValue,
                Group = group
            };
            group.Details.Add(detail);
        }

        public static void CreateDetailForGroup(StatisticsGroup group, int sort, string icon,
            string displayName, UserMeasurementUnitMetric metric)
        {
            var detail = new UserMeasurementUnitStatisticsDetail(metric)
            {
                Sort = sort,
                Icon = icon,
                DisplayDescription = displayName,
                Group = group
            };
            group.Details.Add(detail);
        }

        public static StatisticsGroup CreateGroup(string groupName, int groupSort, string detailIcon, 
            string detailDisplayName, UserMeasurementUnitMetric detailMetric)
        {
            var group = CreateGroup(groupName, groupSort);
            CreateDetailForGroup(group, 0, detailIcon, detailDisplayName, detailMetric);
            return group;
        }

        public static StatisticsGroup CreateGroup(string groupName, int groupSort, string detailIcon,
            string detailDisplayName, string detailDisplayValue)
        {
            var group = CreateGroup(groupName, groupSort);
            CreateDetailForGroup(group, 0, detailIcon, detailDisplayName, detailDisplayValue);
            return group;
        }

        private static void FillGroup(StatisticsGroup group, StatTotals stats)
        {
            CreateDetailForGroup(group, 0, "#", "activities", stats.Count.ToString());
            CreateDetailForGroup(group, 1, "", "moving time",
                $"{Converters.SecToTimeConverter.Convert(stats.MovingTime, typeof(int), null, string.Empty)}");
            CreateDetailForGroup(group, 2, "", "total distance", stats.TotalDistanceUserMeasurementUnit);
            CreateDetailForGroup(group, 3, "", "elevation gain", stats.ElevationGainUserMeasurementUnit);
        }

        public static List<StatisticsGroup> GetRunStatistics(Stats stats)
        {
            bool IsMetric = MeasurementHelper.IsMetric(stats.MeasurementUnit);
            var elevationDistanceUnitType = MeasurementHelper.GetElevationUnitType(IsMetric);
            var distanceUnitType = MeasurementHelper.GetDistanceUnitType(IsMetric);

            var RunStatisticGroups = new List<StatisticsGroup>();

            StatisticsGroup recent = CreateGroup("recent", 0);
            FillGroup(recent, stats.RecentRunTotals);
            RunStatisticGroups.Add(recent);

            StatisticsGroup year = CreateGroup("year total", 1);
            FillGroup(year, stats.YearToDateRunTotals);
            RunStatisticGroups.Add(year);

            StatisticsGroup allTime = CreateGroup("all time total", 2);
            FillGroup(allTime, stats.RunTotals);
            RunStatisticGroups.Add(allTime);
            
            return RunStatisticGroups;
        }

        public static List<StatisticsGroup> GetRideStatistics(Stats stats)
        {
            bool IsMetric = MeasurementHelper.IsMetric(stats.MeasurementUnit);
            var elevationDistanceUnitType = MeasurementHelper.GetElevationUnitType(IsMetric);
            var distanceUnitType = MeasurementHelper.GetDistanceUnitType(IsMetric);

            var RunStatisticGroups = new List<StatisticsGroup>();

            StatisticsGroup recent = CreateGroup("recent", 0);
            FillGroup(recent, stats.RecentRideTotals);
            RunStatisticGroups.Add(recent);

            StatisticsGroup year = CreateGroup("year total", 1);
            FillGroup(year, stats.YearToDateRideTotals);
            RunStatisticGroups.Add(year);

            StatisticsGroup allTime = CreateGroup("all time total", 2);
            FillGroup(allTime, stats.RideTotals);
            RunStatisticGroups.Add(allTime);

            return RunStatisticGroups;
        }
    }
}
