using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels.Special
{
    public class SpecialMovieVM
    {
        public Guid MovieId { get; set; }
        public string MovieTitle { get; set; }
        public string MoviePosterUrl { get; set; }
        public bool IsFeatured { get; set; }
        public int DisplayOrder { get; set; }
    }
}
