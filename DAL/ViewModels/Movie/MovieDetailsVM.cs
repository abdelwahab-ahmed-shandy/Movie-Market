﻿
using MovieMart.Models;

namespace DAL.ViewModels.Movie
{
    public class MovieDetailsVM
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public double Price { get; set; }
        public string? Author { get; set; }
        public string? ImgUrl { get; set; }
        public TimeSpan Duration { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int ReleaseYear { get; set; }
        public double? Rating { get; set; }
        public string CategoryName { get; set; } = null!;

        // Additional details
        public IEnumerable<string> Characters { get; set; } = new List<string>();
        public IEnumerable<CinemaMovieDetailsVM> Cinemas { get; set; } = new List<CinemaMovieDetailsVM>();
        public IEnumerable<string> Specials { get; set; } = new List<string>();
    }

    public class CinemaMovieDetailsVM
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
    }
}
