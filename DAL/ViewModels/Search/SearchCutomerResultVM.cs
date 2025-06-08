using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels.Search
{
    public class SearchCutomerResultVM
    {
        public List<MovieSearchResultVM> Movies { get; set; } = new();
        public List<CinemaSearchResultVM> Cinemas { get; set; } = new();
        public List<CategorySearchResultVM> Categories { get; set; } = new();
        public List<TvSeriesSearchResultVM> TvSeries { get; set; } = new();
        public List<SeasonSearchResultVM> Seasons { get; set; } = new();
        public List<EpisodeSearchResultVM> Episodes { get; set; } = new();
        public List<CharacterSearchResultVM> Characters { get; set; } = new();

        public int TotalResults { get; set; }
    }

    public class MovieSearchResultVM
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public int ReleaseYear { get; set; }
    }

    public class CinemaSearchResultVM
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
    }

    public class CategorySearchResultVM
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class TvSeriesSearchResultVM
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public int ReleaseYear { get; set; }
    }

    public class SeasonSearchResultVM
    {
        public string Title { get; set; }
        public int ReleaseYear { get; set; }
        public int SeasonNumber { get; set; }
    }

    public class EpisodeSearchResultVM
    {
        public string Title { get; set; }
        public int EpisodeNumber { get; set; }
        public TimeSpan Duration { get; set; }
        public string Description { get; set; }
    }

    public class CharacterSearchResultVM
    {
        public string Name { get; set; }
        public string Description { get; set; }

    }

}
