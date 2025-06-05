using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Enums
{
    public enum OrderStatus
    {
        Pending = 0,
        Canceled = 1,
        InProgress = 2,
        Shipped = 3,
        Completed = 4
    }
}
