using MovieMart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class OrderItem : BaseModel
    {
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }
        public Order Order { get; set; }

        public Guid MovieId { get; set; }
        public Movie Movie { get; set; }

        public int Count { get; set; }
        public double Price { get; set; }

        [NotMapped] // This prevents EF from trying to map it to the database
        public Cinema Cinema { get; set; }
    }
}
