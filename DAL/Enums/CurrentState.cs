using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Enums
{
    public enum CurrentState
    {
        Inactive = 0,
        Active = 1,
        Archived = 2,
        SoftDelete = 3,
        RestoreDeleted = 4,
        Update = 5
    }
}
