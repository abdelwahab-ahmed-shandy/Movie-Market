

namespace MovieMart.Models
{
    [Index(nameof(Title), IsUnique = false)]
    public class Movie : BaseModel
    {
        [Required, MaxLength(200)]
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        [Required, Range(0, 1000)]
        public double Price { get; set; }

        [MaxLength(100)]
        public string? Author { get; set; }
        public string? ImgUrl { get; set; }

        public TimeSpan Duration { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        [Range(1900, 2100)]
        public int ReleaseYear { get; set; }

        [Range(0, 10)]
        public double? Rating { get; set; }

        // Relationships
        public Guid CategoryId { get; set; }
        public virtual Category Category { get; set; } = null!;

        // Many-to-Many: Cinema <-> Movie
        public virtual ICollection<CinemaMovie> CinemaMovies { get; set; } = new List<CinemaMovie>();

        // Many-to-Many: Character <-> Movie
        public virtual ICollection<CharacterMovie> CharacterMovies { get; set; } = new List<CharacterMovie>();


        // Many-to-Many: Cinema <-> Special
        public virtual ICollection<MovieSpecial> MovieSpecials { get; set; } = new List<MovieSpecial>();

    }
}
