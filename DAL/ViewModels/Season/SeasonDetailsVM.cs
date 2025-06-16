using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels.Season
{
    public class SeasonDetailsVM
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public int SeasonNumber { get; set; }
        public int ReleaseYear { get; set; }
        //public string? Description { get; set; }
        public string TvSeriesTitle { get; set; }
        public Guid TvSeriesId { get; set; }
        public string? ImgUrl { get; set; }
        public List<EpisodeCustomerVM> Episodes { get; set; } = new List<EpisodeCustomerVM>();
    }
}
