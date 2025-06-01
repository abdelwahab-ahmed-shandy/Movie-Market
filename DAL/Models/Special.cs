
namespace MovieMart.Models
{
    public class Special
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Range(0, 100)]
        public decimal DiscountPercentage { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        // Many-to-Many with Movie
        public ICollection<MovieSpecial> MovieSpecials { get; set; }

    }
}
