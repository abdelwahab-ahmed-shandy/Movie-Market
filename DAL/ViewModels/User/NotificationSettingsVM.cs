using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels.User
{
    public class NotificationSettingsVM
    {
        public bool ReceiveMovieUpdates { get; set; }
        public bool ReceivePromotions { get; set; }
        public bool ReceiveSystemNotifications { get; set; }
    }
}
