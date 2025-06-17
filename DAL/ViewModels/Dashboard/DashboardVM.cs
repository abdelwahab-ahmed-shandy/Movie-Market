namespace DAL.ViewModels.Dashboard
{
    public class DashboardVM
    {
        public int TotalMovies { get; set; }
        public int TotalCinemas { get; set; }
        public int TotalTvSeries { get; set; }
        public int TotalCategories { get; set; }
        public int TotalCharacters { get; set; }
        public int TotalEpisodes { get; set; }
        public int TotalSeasons { get; set; }

        public IEnumerable<MovieMart.Models.Movie> RecentMovies { get; set; }
        public IEnumerable<MovieMart.Models.Movie> NewReleases { get; set; }
        public IEnumerable<MovieMart.Models.Movie> TopRated { get; set; }
        public IEnumerable<MovieMart.Models.Cinema> RecentCinemas { get; set; }
        public IEnumerable<MovieMart.Models.TvSeries> RecentTvSeries { get; set; }
        public IEnumerable<MovieMart.Models.TvSeries> PopularSeries { get; set; }
        public IEnumerable<MovieMart.Models.Category> MovieCategories { get; set; }
    }
}
