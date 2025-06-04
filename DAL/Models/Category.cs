
namespace MovieMart.Models
{
    public class Category : BaseModel
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        // One-to-Many: Movie <-> Category
        public ICollection<Movie> Movies { get; set; } = new List<Movie>();
    }
}
