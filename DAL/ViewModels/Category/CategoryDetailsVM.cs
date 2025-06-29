
namespace DAL.ViewModels.Category
{
    public class CategoryDetailsVM
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public IEnumerable<MovieCharacterIndexVM> Movies { get; set; } = new List<MovieCharacterIndexVM>();
    }
}
