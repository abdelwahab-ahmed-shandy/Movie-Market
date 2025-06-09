using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels.Movie
{
    public class MovieAdminListVM
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string? ImgUrl { get; set; }
        public double Price { get; set; }
        public double? Rating { get; set; }
        public string CategoryName { get; set; } = null!;
        public bool IsDeleted { get; set; }
    }
}
