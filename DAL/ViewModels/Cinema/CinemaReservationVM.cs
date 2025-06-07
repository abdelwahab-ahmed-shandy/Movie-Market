using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels.Cinema
{
    public class CinemaReservationVM
    {
        public Guid MovieId { get; set; }
        public string MovieTitle { get; set; }
        public string CinemaName { get; set; }
        public DateTime ShowTime { get; set; }
        public double Price { get; set; }
        public string MovieImage { get; set; }

        public string FormattedShowTime => ShowTime.ToString("dd MMM yyyy, hh:mm tt");
        public string ShortCinemaName => CinemaName.Length > 15 ?
            CinemaName.Substring(0, 15) + "..." : CinemaName;
    }
}
