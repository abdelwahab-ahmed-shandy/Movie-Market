using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels.Season
{
    public class SeasonAdminDetailsVM
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public int SeasonNumber { get; set; }
        public int ReleaseYear { get; set; }
        public bool IsDeleted { get; set; }
        public CurrentState CurrentState { get; set; }
        public string TvSeriesTitle { get; set; }
        public Guid TvSeriesId { get; set; }
        public List<EpisodeSeasonAdminVM> Episodes { get; set; } = new List<EpisodeSeasonAdminVM>();
        public DateTime CreatedDateUtc { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? LastModifiedDateUtc { get; set; }
        public string? LastModifiedBy { get; set; }
    }
}
