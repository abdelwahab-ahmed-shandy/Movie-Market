using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels.Season
{
    public class EpisodeCustomerVM
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public int EpisodeNumber { get; set; }
        public string? Description { get; set; }
        public TimeSpan Duration { get; set; }
        public string? ThumbnailUrl { get; set; }
        public DateTime? ReleaseDate { get; set; }
    }
}
