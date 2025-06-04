namespace MovieMart.Models
{
    public class CharacterMovie : Movie
    {
        // Many-to-Many: Character <-> Movie
        public Guid CharacterId { get; set; }
        public Character Character { get; set; } = null!;

        public Guid MovieId { get; set; }
        public Movie Movie { get; set; } = null!;
    }
}
