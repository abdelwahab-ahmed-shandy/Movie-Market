using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels.Movie
{
    public class MovieAdminCreateVM
    {
        [Required, MaxLength(200)]
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        [Required, Range(0, 1000)]
        public double Price { get; set; }

        [MaxLength(100)]
        public string? Author { get; set; }

        [Display(Name = "Image URL")]
        public string? ImgUrl { get; set; }

        [Display(Name = "Duration (hh:mm:ss)")]
        public TimeSpan Duration { get; set; }

        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "End Date")]
        public DateTime? EndDate { get; set; }

        [Display(Name = "Release Year")]
        [Range(1900, 2100)]
        public int ReleaseYear { get; set; }

        [Range(0, 10)]
        public double? Rating { get; set; }

        [Display(Name = "Category")]
        public Guid CategoryId { get; set; }

        [Display(Name = "Characters")]
        public List<Guid> CharacterIds { get; set; } = new();

        [Display(Name = "Cinemas")]
        public List<Guid> CinemaIds { get; set; } = new();

        [Display(Name = "Specials")]
        public List<Guid> SpecialIds { get; set; } = new();

        public CurrentState CurrentState { get; set; }
    }
}
