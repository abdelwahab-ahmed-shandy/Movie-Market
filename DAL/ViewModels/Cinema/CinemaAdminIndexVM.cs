using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels.Cinema
{
    public class CinemaAdminIndexVM
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Location { get; set; } = string.Empty;
        public CurrentState CurrentState { get; set; }
        public DateTime CreatedDateUtc { get; set; }
        public int CinemasCount { get; set; }
        public bool IsDeleted { get; set; }
    }
}
