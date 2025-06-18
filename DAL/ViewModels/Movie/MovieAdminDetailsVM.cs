using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels.Movie
{
    public class MovieAdminDetailsVM
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public CurrentState CurrentState { get; set; }
        public double Price { get; set; }
        public string? Author { get; set; }
        public string? ImgUrl { get; set; }
        public TimeSpan Duration { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int ReleaseYear { get; set; }
        public double? Rating { get; set; }

        public CategoryMovieVM? Category { get; set; }
        public List<CharacterMovieVM> Characters { get; set; } = new();
        public List<CinemaMovieVM> Cinemas { get; set; } = new();
        public List<SpecialMovieViewModel> Specials { get; set; } = new();

        public string? CreatedBy { get; set; }
        public DateTime CreatedDateUtc { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDateUtc { get; set; }
        public string? DeletedBy { get; set; }
        public DateTime? DeletedDateUtc { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class CategoryMovieVM
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class CharacterMovieVM
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
    }

    public class CinemaMovieVM
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public DateTime ShowTime { get; set; }
    }

    public class SpecialMovieViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public bool IsFeatured { get; set; }
        public int DisplayOrder { get; set; }
    }
}
