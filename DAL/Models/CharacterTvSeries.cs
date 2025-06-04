namespace MovieMart.Models
{
    public class CharacterTvSeries : BaseModel
    {
        // Many-to-Many: Character <-> TvSeries

        public Guid CharacterId { get; set; }
        public Character Character { get; set; } = null!;

        public Guid TvSeriesId { get; set; }
        public TvSeries TvSeries { get; set; } = null!;
    }
}
