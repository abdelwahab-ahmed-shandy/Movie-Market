using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels.TvSeries
{
    public class TvSeriesVM
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string? Author { get; set; }
        public string? ImgUrl { get; set; }
        public int ReleaseYear { get; set; }
        public double? Rating { get; set; }
        public string? ShortDescription => Description?.Length > 100 ?
            Description.Substring(0, 100) + "..." : Description;
    }
}
