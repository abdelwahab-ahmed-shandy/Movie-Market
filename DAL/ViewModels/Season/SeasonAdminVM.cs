using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels.Season
{
    public class SeasonAdminVM
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public int SeasonNumber { get; set; }
        public int ReleaseYear { get; set; }
        public int EpisodeCount { get; set; }
        public string TvSeriesTitle { get; set; }
        public bool IsDeleted { get; set; }
        public CurrentState CurrentState { get; set; }
    }
}
