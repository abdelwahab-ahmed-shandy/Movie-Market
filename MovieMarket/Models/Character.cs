using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace MovieMart.Models
{
    public class Character
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; } = null!;
        [StringLength(500, MinimumLength = 8)]
        public string? Description { get; set; }

        // Many-to-Many relationship with Movie via CharacterMovie
        [ValidateNever]
        public ICollection<CharacterMovie> CharacterMovies { get; set; } = new List<CharacterMovie>();
        [ValidateNever]
        public ICollection<CharacterTvSeries> CharacterTvSeries { get; set; } = new List<CharacterTvSeries>();
    }
}
