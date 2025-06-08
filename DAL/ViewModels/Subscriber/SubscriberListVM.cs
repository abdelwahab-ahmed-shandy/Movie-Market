using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels.Subscriber
{
    public class SubscriberListVM
    {
        public IEnumerable<SubscriberVM> Subscribers { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public string SearchTerm { get; set; }

    }
}
