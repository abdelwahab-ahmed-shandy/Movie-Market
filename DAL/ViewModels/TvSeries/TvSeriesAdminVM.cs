using DAL.ViewModels.Character;
using DAL.ViewModels.Season;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels.TvSeries
{
    public class TvSeriesAdminVM
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? ImgUrl { get; set; }
        public int ReleaseYear { get; set; }
        public double? Rating { get; set; }
        public int SeasonCount { get; set; }
        public bool IsDeleted { get; set; }
        public CurrentState CurrentState { get; set; }

    }
}
