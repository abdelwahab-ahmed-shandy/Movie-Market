using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using MovieMarket.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieMart.Models
{
    public class Movie
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Title { get; set; }

        [StringLength(500, MinimumLength = 8)]
        public string? Description { get; set; }

        [Required]
        public double Price { get; set; }

        [StringLength(100, MinimumLength = 3)]
        public string? Author { get; set; }

        [RegularExpression(@"^.*\.(jpg|jpeg|png)$")]
        public string? ImgUrl { get; set; }

        [Required]
        [Range(typeof(TimeSpan), "00:01:00", "23:59:59")]
        public TimeSpan Duration { get; set; }

        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        [Required]
        [Range(1888, 2100)]
        public int ReleaseYear { get; set; }

        [Range(0, 10)]
        public double? Rating { get; set; }

        // One-to-Many: Movie <-> Category
        [Required]
        public int CategoryId { get; set; }
        [ValidateNever]
        public Category Category { get; set; } = null!;

        // Many-to-Many: Character <-> Movie
        [ValidateNever]
        public ICollection<CharacterMovie> CharacterMovies { get; set; } = new List<CharacterMovie>();

        // Many-to-Many: Cinema <-> Movie
        [ValidateNever]
        public ICollection<CinemaMovie> CinemaMovies { get; set; } = new List<CinemaMovie>();
    }
}
