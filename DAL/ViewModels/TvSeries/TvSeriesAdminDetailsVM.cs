using DAL.ViewModels.Character;
using DAL.ViewModels.Season;
using MovieMart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels.TvSeries
{
    public class TvSeriesAdminDetailsVM
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string? Author { get; set; }
        public string? ImgUrl { get; set; }
        public int ReleaseYear { get; set; }
        public double? Rating { get; set; }
        public bool IsDeleted { get; set; }
        public CurrentState CurrentState { get; set; }
        public DateTime CreatedDateUtc { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? LastModifiedDateUtc { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? DeletedDateUtc { get; set; }
        public string? DeletedBy { get; set; }

        public List<SeasonTvSeriesVM> Seasons { get; set; } = new List<SeasonTvSeriesVM>();
        public List<CharacterTvSeriesVM> Characters { get; set; } = new List<CharacterTvSeriesVM>();
    }
}
