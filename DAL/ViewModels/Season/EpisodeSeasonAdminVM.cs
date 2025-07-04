using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels.Season
{
    public class EpisodeSeasonAdminVM
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public int EpisodeNumber { get; set; }
        public bool IsDeleted { get; set; }
    }
}
