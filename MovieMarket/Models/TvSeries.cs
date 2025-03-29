using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace MovieMart.Models
{
    public class TvSeries
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Title { get; set; }
        [StringLength(500, MinimumLength = 8)]
        public string? Description { get; set; }
        [StringLength(100, MinimumLength = 3)]
        public string? Author { get; set; }
        [RegularExpression(@"^.*\.(jpg|jpeg|png)$")]
        public string? ImgUrl { get; set; }
        [Required]
        [Range(1888, 2100)]
        public int ReleaseYear { get; set; }
        [Range(0, 10)]
        public double? Rating { get; set; }

        // One-to-Many: TvSeries <-> Season
        [ValidateNever]
        public ICollection<Models.Season> Seasons { get; set; } = new List<Season>();
        [ValidateNever]
        public ICollection<Models.CharacterTvSeries> Characters { get; set; } = new List<CharacterTvSeries>();

    }
}
