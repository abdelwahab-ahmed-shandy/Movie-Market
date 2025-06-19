using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels.Movie
{
    public class MovieAdminEditVM : MovieAdminCreateVM
    {
        public Guid Id { get; set; }
        //public CurrentState CurrentState { get; set; }
        public Dictionary<Guid, bool> SpecialFeatures { get; set; } = new(); // SpecialId -> IsFeatured
        public Dictionary<Guid, int> SpecialDisplayOrders { get; set; } = new(); // SpecialId -> DisplayOrder
        public Dictionary<Guid, DateTime> CinemaShowTimes { get; set; } = new(); // CinemaId -> ShowTime

    }
}
