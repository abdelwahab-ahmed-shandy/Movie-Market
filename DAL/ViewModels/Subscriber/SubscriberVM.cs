using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels.Subscriber
{
    public class SubscriberVM
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public DateTime SubscribedAt { get; set; }
    }
}
