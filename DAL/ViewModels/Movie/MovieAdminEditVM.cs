using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels.Movie
{
    public class MovieAdminEditVM 
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public double Price { get; set; }
        public string? Author { get; set; }
        public TimeSpan Duration { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int ReleaseYear { get; set; }
        public double? Rating { get; set; }
        public Guid CategoryId { get; set; }
        public List<Guid> CharacterIds { get; set; } = new();
        public List<Guid> CinemaIds { get; set; } = new();
        public List<Guid> SpecialIds { get; set; } = new();
        public CurrentState CurrentState { get; set; }

        [Display(Name = "Movie Icon")]
        [DataType(DataType.Upload)]
        public IFormFile? ImgFile { get; set; }
    }
}
