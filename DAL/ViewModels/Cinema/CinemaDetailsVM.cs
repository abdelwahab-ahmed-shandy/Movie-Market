using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels.Cinema
{
    public class CinemaDetailsVM
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? Img { get; set; } 
        public List<MovieDetailsVM> Movies { get; set; } = new();
        public int FilteredMoviesCount => Movies.Count;

    }
}
