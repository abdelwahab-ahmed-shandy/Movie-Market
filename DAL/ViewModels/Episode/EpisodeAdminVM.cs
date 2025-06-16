using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels.Episode
{
    public class EpisodeAdminVM
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public int EpisodeNumber { get; set; }
        public string SeasonTitle { get; set; }
        public string TvSeriesTitle { get; set; }
        public TimeSpan Duration { get; set; }
        public decimal? Rating { get; set; }
        public bool IsDeleted { get; set; }
        public CurrentState CurrentState { get; set; }
    }
}
