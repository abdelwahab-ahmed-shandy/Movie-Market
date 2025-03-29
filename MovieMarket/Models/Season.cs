using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace MovieMart.Models
{
    public class Season
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Title { get; set; }
        [Required]
        [Range(1888, 2100)]
        public int ReleaseYear { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public int SeasonNumber { get; set; }

        // One-to-Many: TvSeries <-> Season
        [Required]
        public int TvSeriesId { get; set; }
        [ValidateNever]
        public TvSeries TvSeries { get; set; } = null!;

        // One-to-Many: Season <-> Episode
        [ValidateNever]
        public ICollection<Models.Episode> Episodes { get; set; } = new List<Episode>();
    }
}
