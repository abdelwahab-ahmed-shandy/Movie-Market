namespace MovieMarket.Models.ViewModels
{
    public class CinemaViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Location { get; set; } = string.Empty;

        // For multiple selected movies
        public List<int> SelectedMovieIds { get; set; } = new();
    }
}
