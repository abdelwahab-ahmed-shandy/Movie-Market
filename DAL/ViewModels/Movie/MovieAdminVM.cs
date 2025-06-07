using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels.Movie
{
    public class MovieAdminVM
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public CurrentState CurrentState { get; set; }
        public bool IsDeleted { get; set; }
    }
}
