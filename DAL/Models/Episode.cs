
namespace MovieMart.Models
{
    public class Episode
    {
        public int Id { get; set; }

        [Required]
        public int EpisodeNumber { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Title { get; set; }

        public string? Description { get; set; } = string.Empty;
        public decimal? Rating { get; set; } = 5.5m;

        [Required]
        public TimeSpan Duration { get; set; }

        // Add the episode link here
        [Url]
        public string? VideoUrl { get; set; }

        [Display(Name = "Thumbnail URL")]
        [Url(ErrorMessage = "Invalid URL format")]
        public string? ThumbnailUrl { get; set; }

        // One-to-Many: Season <-> Episode
        [Required]
        public int SeasonId { get; set; }
        public Season Season { get; set; } = null!;

    }
}
