namespace MovieMart.Models
{
    public class CinemaMovie : BaseModel
    {
        public Guid CinemaId { get; set; }
        public Cinema Cinema { get; set; } = null!;

        public Guid MovieId { get; set; }
        public Movie Movie { get; set; } = null!;

        public DateTime ShowTime { get; set; }
    }
}
