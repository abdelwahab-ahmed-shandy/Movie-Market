using MovieMart.Models;

namespace MovieMart.Models
{
    [PrimaryKey(nameof(MovieId), nameof(CinemaId), nameof(ApplicationUserId))]
    public class Cart
    {
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public int MovieId { get; set; }
        public Movie Movie { get; set; }

        public int CinemaId { get; set; }
        public Cinema Cinema { get; set; }

        public int Count { get; set; }

    }
}
