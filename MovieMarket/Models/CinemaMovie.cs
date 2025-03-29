namespace MovieMarket.Models
{
    public class CinemaMovie
    {
        public int Id { get; set; }
        // FK for the Many-to-Many relationship between Cinema and Movie
        public int CinemaId { get; set; }
        public Cinema Cinema { get; set; } = null!;

        public int MovieId { get; set; }
        public Movie Movie { get; set; } = null!;

        [Required]
        public DateTime ShowTime { get; set; }
    }
}
