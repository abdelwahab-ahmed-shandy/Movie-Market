
namespace BLL.Services.Interfaces
{
    public interface INewsletterService
    {
        Task<SubscriberListVM> GetSubscribersAsync(string query, int page, int pageSize);
        Task<(bool Success, string Message)> SendNewsletterAsync(string subject, string body);
        Task<List<SentEmail>> GetEmailHistoryAsync();
        Task DeleteSubscriberAsync(Guid id);

    }
}
