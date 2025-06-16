using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels.Episode
{
    public class EpisodeAdminEditVM
    {
        public Guid Id { get; set; }

        [Required]
        [Range(1, 1000)]
        public int EpisodeNumber { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [Range(0, 10)]
        public decimal? Rating { get; set; }

        [Required]
        public TimeSpan Duration { get; set; }

        [Url]
        public string? VideoUrl { get; set; }

        [Url]
        public string? ThumbnailUrl { get; set; }

        public CurrentState CurrentState { get; set; }
    }
}
