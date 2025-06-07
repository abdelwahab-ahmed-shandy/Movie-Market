using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels.Movie
{
    public class MovieVM
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? PosterUrl { get; set; }
        public int ReleaseYear { get; set; }
        public double? Rating { get; set; }
        public string? ShortDescription { get; set; }

    }
}
