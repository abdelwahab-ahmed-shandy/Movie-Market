using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels.Cinema
{
    public class CinemaAdminDetailsVM
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Location { get; set; } = string.Empty;
        public CurrentState CurrentState { get; set; }

        // Audit Properties
        public string CreatedBy { get; set; }
        public DateTime CreatedDateUtc { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDateUtc { get; set; }
        public string? DeletedBy { get; set; }
        public DateTime? DeletedDateUtc { get; set; }
        public IEnumerable<MovieAdminVM> Movies { get; set; } = new List<MovieAdminVM>();

    }
}
