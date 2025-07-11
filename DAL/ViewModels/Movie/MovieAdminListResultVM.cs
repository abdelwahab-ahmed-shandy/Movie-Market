﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels.Movie
{
    public class MovieAdminListResultVM
    {
        public IEnumerable<MovieAdminListVM> Movies { get; set; } = Enumerable.Empty<MovieAdminListVM>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SearchTerm { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }

}
