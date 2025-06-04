namespace MovieMart.Models
{
    public class MovieSpecial : BaseModel
    {
        public Guid ParentMovieId { get; set; }
        public Movie ParentMovie { get; set; } = null!;

        public Guid SpecialId { get; set; }
        public Special Special { get; set; } = null!;

        public bool IsFeatured { get; set; }
        public int DisplayOrder { get; set; }
    }
}
