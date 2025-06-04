
namespace MovieMart.Models
{
    public class Character : BaseModel
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        // Many-to-Many relationship with Movie via CharacterMovie
        public ICollection<CharacterMovie> CharacterMovies { get; set; } = new List<CharacterMovie>();
        // Many-to-Many relationship with TvSeries via CharacterTvSeries
        public ICollection<CharacterTvSeries> CharacterTvSeries { get; set; } = new List<CharacterTvSeries>();
    }
}
