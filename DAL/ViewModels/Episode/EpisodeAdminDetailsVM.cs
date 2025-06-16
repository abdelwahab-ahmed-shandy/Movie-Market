using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels.Episode
{
    public class EpisodeAdminDetailsVM
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public int EpisodeNumber { get; set; }
        public string? Description { get; set; }
        public decimal? Rating { get; set; }
        public TimeSpan Duration { get; set; }
        public string? VideoUrl { get; set; }
        public string? ThumbnailUrl { get; set; }
        public bool IsDeleted { get; set; }
        public CurrentState CurrentState { get; set; }
        public string SeasonTitle { get; set; }
        public Guid SeasonId { get; set; }
        public string TvSeriesTitle { get; set; }
        public Guid TvSeriesId { get; set; }
        public DateTime CreatedDateUtc { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? LastModifiedDateUtc { get; set; }
        public string? LastModifiedBy { get; set; }
    }
}
