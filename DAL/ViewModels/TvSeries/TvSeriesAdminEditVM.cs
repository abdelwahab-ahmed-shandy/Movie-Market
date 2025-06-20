using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels.TvSeries
{
    public class TvSeriesAdminEditVM
    {
        [Display(Name = "Status")]
        public CurrentState CurrentState { get; set; }

        public Guid Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string? Description { get; set; }
        public string? Author { get; set; }

        [Required]
        [Display(Name = "Release Year")]
        public int ReleaseYear { get; set; }
        [DataType(DataType.Upload)]
        public IFormFile? IconFile { get; set; } 
    }
}
