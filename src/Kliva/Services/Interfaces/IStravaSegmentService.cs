using System.Collections.Generic;
using System.Threading.Tasks;
using Kliva.Models;

namespace Kliva.Services.Interfaces
{
    public interface IStravaSegmentService
    {
        Task<Segment> GetSegmentAsync(string segmentId);
        Task<SegmentEffort> GetSegmentEffortAsync(string segmentEffortId);
        Task<List<SegmentSummary>> GetStarredSegmentsAsync();
        Task<List<SegmentSummary>> GetStarredSegmentsAsync(string athleteId);
        Task<Leaderboard> GetLeaderBoardAsync(string segmentId, bool following = false);
        void FillStatistics(SegmentEffort segmentEffort, Leaderboard leaderboard);
    }
}
