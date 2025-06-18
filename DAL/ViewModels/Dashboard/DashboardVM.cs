namespace DAL.ViewModels.Dashboard
{
    public class DashboardVM
    {
        public IEnumerable<MovieMart.Models.Movie> RecentMovies { get; set; }
        public IEnumerable<MovieMart.Models.Movie> NewReleases { get; set; }
        public IEnumerable<MovieMart.Models.Movie> TopRated { get; set; }
        public IEnumerable<MovieMart.Models.Cinema> RecentCinemas { get; set; }
        public IEnumerable<MovieMart.Models.TvSeries> RecentTvSeries { get; set; }
        public IEnumerable<MovieMart.Models.TvSeries> PopularSeries { get; set; }
        public IEnumerable<MovieMart.Models.Category> MovieCategories { get; set; }
    }
}
