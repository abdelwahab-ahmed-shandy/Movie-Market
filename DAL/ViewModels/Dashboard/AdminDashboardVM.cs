
namespace DAL.ViewModels.Dashboard
{
    public class AdminDashboardVM
    {
        public int TotalUsers { get; set; }
        public int TotalActiveSpecials { get; set; }
        public int TotalCharacters { get; set; }
        public int TotalCategories { get; set; }
        public int TotalMovies { get; set; }
        public int TotalCinemas { get; set; }
        public int TotalTvSeries { get; set; }
        public int TotalOrders { get; set; }
        public List<Order> RecentOrders { get; set; } = new List<Order>();
        public List<MovieMart.Models.Subscriber> RecentSubscriber { get; set; } = new List<MovieMart.Models.Subscriber>();
    }
}
