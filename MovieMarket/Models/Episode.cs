using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

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
        [Required]
        public TimeSpan Duration { get; set; }

        // Add the episode link here
        [Url]
        public string? VideoUrl { get; set; }
        // One-to-Many: Season <-> Episode
        [Required]
        public int SeasonId { get; set; }
        [ValidateNever]
        public Season Season { get; set; } = null!;

    }
}
