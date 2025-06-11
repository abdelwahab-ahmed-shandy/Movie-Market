
namespace DAL.ViewModels.Category
{
    public class CategoryIndexVM
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? IconUrl { get; set; }
        public int? MoviesCount { get; set; }
    }
}
