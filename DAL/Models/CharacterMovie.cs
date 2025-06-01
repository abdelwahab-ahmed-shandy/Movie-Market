namespace MovieMart.Models
{
    public class CharacterMovie
    {
        // Many-to-Many: Character <-> Movie
        public int CharacterId { get; set; }
        public Character Character { get; set; } = null!;

        public int MovieId { get; set; }
        public Movie Movie { get; set; } = null!;
    }
}
