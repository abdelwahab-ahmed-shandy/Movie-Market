using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels.Season
{
    public class SeasonAdminCreateVM
    {
        [Required]
        public Guid TvSeriesId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        [Range(1, 100)]
        public int SeasonNumber { get; set; }

        [Required]
        [Range(1900, 2100)]
        public int ReleaseYear { get; set; }
    }
}
