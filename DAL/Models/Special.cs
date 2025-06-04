
namespace MovieMart.Models
{
    public class Special : BaseModel
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal DiscountPercentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // Many-to-Many with Movie
        public ICollection<MovieSpecial> MovieSpecials { get; set; }

    }
}
