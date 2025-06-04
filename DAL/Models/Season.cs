
namespace MovieMart.Models
{
    public class Season : BaseModel
    {
        public string Title { get; set; }
        public int ReleaseYear { get; set; }
        public int SeasonNumber { get; set; }
        // One-to-Many: TvSeries <-> Season
        [Required]
        public Guid TvSeriesId { get; set; }
        public TvSeries TvSeries { get; set; } = null!;

        // One-to-Many: Season <-> Episode
        public ICollection<Episode> Episodes { get; set; } = new List<Episode>();
    }
}
