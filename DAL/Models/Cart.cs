using MovieMart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    //[PrimaryKey(nameof(MovieId), nameof(CinemaId), nameof(ApplicationUserId))]
    public class Cart : BaseModel
    {
        public Guid ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public Guid MovieId { get; set; }
        public Movie Movie { get; set; }

        public Guid CinemaId { get; set; }
        public Cinema Cinema { get; set; }

        public CartStatus CartStatus { get; set; }
        public int Count { get; set; }
    }

}
