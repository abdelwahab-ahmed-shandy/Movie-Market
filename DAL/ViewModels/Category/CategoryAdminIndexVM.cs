using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels.Category
{
    public class CategoryAdminIndexVM
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public CurrentState CurrentState { get; set; }
        public DateTime CreatedDateUtc { get; set; }
        public int MoviesCount { get; set; }
        public bool IsDeleted { get; set; }
    }
}
