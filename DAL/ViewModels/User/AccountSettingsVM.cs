using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels.User
{
    public class AccountSettingsVM
    {
        public bool TwoFactorEnabled { get; set; }
        public bool EmailNotificationsEnabled { get; set; }
    }
}
