using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace MovieMart.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; } = null!;
        [StringLength(500, MinimumLength = 8)]
        public string? Description { get; set; }

        // One-to-Many: Movie <-> Category
        [ValidateNever]
        public ICollection<Models.Movie> Movies { get; set; } = new List<Movie>();
    }
}
