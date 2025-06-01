namespace MovieMart.Models
{
    public class MovieSpecial
    {
        public int MovieId { get; set; }
        public Movie Movie { get; set; } = null!;

        public int SpecialId { get; set; }
        public Special Special { get; set; } = null!;

        public bool IsFeatured { get; set; }
        public int DisplayOrder { get; set; }
    }
}
