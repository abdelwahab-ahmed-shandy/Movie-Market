using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels.User
{
    public class ChangeUserRoleVM
    {
        public Guid UserId { get; set; }
        public string CurrentEmail { get; set; }
        public string CurrentRole { get; set; }
        public string NewRole { get; set; }
    }
}
