namespace MovieMart.Models
{
    public class CharacterTvSeries
    {
        // Many-to-Many: Character <-> TvSeries

        public int CharacterId { get; set; }
        public Character Character { get; set; } = null!;

        public int TvSeriesId { get; set; }
        public TvSeries TvSeries { get; set; } = null!;
    }
}
