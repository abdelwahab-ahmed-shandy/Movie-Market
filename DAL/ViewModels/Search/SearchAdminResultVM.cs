using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels.Search
{
    public class SearchAdminResultVM
    {
        public List<MovieAdminSearchResultVM> Movies { get; set; } = new();
        public List<CinemaAdminSearchResultVM> Cinemas { get; set; } = new();
        public List<CategoryAdminSearchResultVM> Categories { get; set; } = new();
        public List<TvSeriesAdminSearchResultVM> TvSeries { get; set; } = new();
        public List<SeasonAdminSearchResultVM> Seasons { get; set; } = new();
        public List<EpisodeAdminSearchResultVM> Episodes { get; set; } = new();
        public List<CharacterAdminSearchResultVM> Characters { get; set; } = new();

        public int TotalResults { get; set; }
    }

    public class BaseAdminModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public CurrentState? CurrentState { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDateUtc { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDateUtc { get; set; }
        public string? BlockedBy { get; set; }
        public DateTime? BlockedDateUtc { get; set; }
        public string? DeletedBy { get; set; }
        public DateTime? DeletedDateUtc { get; set; }
        public string? RestoredBy { get; set; }
        public DateTime? RestoredDateUtc { get; set; }
    }

    public class MovieAdminSearchResultVM
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public int ReleaseYear { get; set; }
        public double Price { get; set; }

        public List<BaseAdminModel> Bases { get; set; } = new List<BaseAdminModel>();
    }

    public class CinemaAdminSearchResultVM
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }

        public List<BaseAdminModel> Bases { get; set; } = new List<BaseAdminModel>();

    }

    public class CategoryAdminSearchResultVM
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<BaseAdminModel> Bases { get; set; } = new List<BaseAdminModel>();

    }

    public class TvSeriesAdminSearchResultVM
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public int ReleaseYear { get; set; }
        public List<BaseAdminModel> Bases { get; set; } = new List<BaseAdminModel>();

    }

    public class SeasonAdminSearchResultVM
    {
        public string Title { get; set; }
        public int ReleaseYear { get; set; }
        public int SeasonNumber { get; set; }
        public List<BaseAdminModel> Bases { get; set; } = new List<BaseAdminModel>();

    }

    public class EpisodeAdminSearchResultVM
    {
        public string Title { get; set; }
        public int EpisodeNumber { get; set; }
        public TimeSpan Duration { get; set; }
        public string Description { get; set; }
        public List<BaseAdminModel> Bases { get; set; } = new List<BaseAdminModel>();

    }

    public class CharacterAdminSearchResultVM
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<BaseAdminModel> Bases { get; set; } = new List<BaseAdminModel>();

    }

}

