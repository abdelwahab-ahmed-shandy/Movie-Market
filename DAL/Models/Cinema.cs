
namespace MovieMart.Models
{
    public class Cinema
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        [StringLength(200)]
        public string Location { get; set; } = string.Empty;

        // Many-to-Many relationship with Movie via CinemaMovies
        public ICollection<CinemaMovie> CinemaMovies { get; set; } = new List<CinemaMovie>();

    }
}
