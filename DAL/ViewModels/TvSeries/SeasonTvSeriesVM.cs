using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels.TvSeries
{
    public class SeasonTvSeriesVM
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public int SeasonNumber { get; set; }
        public int EpisodeCount { get; set; }
        public bool IsDeleted { get; set; }
        public CurrentState CurrentState { get; set; }
    }
}
