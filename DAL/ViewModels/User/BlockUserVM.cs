using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels.User
{
    public class BlockUserVM
    {
        public Guid UserId { get; set; }
        public string BlockReason { get; set; }
    }
}
