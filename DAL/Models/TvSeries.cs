
namespace MovieMart.Models
{
    public class TvSeries : BaseModel
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public string? Author { get; set; }
        public string? ImgUrl { get; set; }
        public int ReleaseYear { get; set; }
        public double? Rating { get; set; }

        // One-to-Many: TvSeries <-> Season
        public ICollection<Models.Season> Seasons { get; set; } = new List<Season>();
        public ICollection<Models.CharacterTvSeries> Characters { get; set; } = new List<CharacterTvSeries>();

    }
}
