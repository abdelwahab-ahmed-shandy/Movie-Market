using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Enums
{
    public enum AccountStateType
    {
        Active = 1,
        PendingActivation = 2,
        Banned = 3,
        Blocked = 4,
        Deleted = 5
    }
}
