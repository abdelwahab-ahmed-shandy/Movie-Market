
namespace MovieMart.Models
{
    public class Cinema : BaseModel
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Location { get; set; } = string.Empty;
        public string? ImgUrl { get; set; }

        // Many-to-Many relationship with Movie via CinemaMovies
        public ICollection<CinemaMovie> CinemaMovies { get; set; } = new List<CinemaMovie>();

    }
}
