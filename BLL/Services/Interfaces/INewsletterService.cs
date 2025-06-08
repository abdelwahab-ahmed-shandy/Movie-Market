using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface INewsletterService
    {
        Task<PaginatedList<Subscriber>> GetSubscribersAsync(string query, int page, int pageSize);
        Task<(bool Success, string Message)> SendNewsletterAsync(string subject, string body);
        Task<List<SentEmail>> GetEmailHistoryAsync();
    }
}
