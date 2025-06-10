using BLL.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces.Customer
{
    public interface ISubscriberService
    {
        Task<(bool Success, string Message)> SubscribeAsync(string email);
    }
}
