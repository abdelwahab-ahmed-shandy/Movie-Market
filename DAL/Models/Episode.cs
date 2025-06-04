
namespace MovieMart.Models
{
    public class Episode : BaseModel
    {
        public int EpisodeNumber { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; } = string.Empty;
        public decimal? Rating { get; set; } = 5.5m;
        public TimeSpan Duration { get; set; }
        public string? VideoUrl { get; set; }
        public string? ThumbnailUrl { get; set; }

        // One-to-Many: Season <-> Episode
        public Guid SeasonId { get; set; }
        public Season Season { get; set; } = null!;

    }
}
