namespace BLL.Services.Interfaces
{
    public interface ISubscriberService
    {
        Task<(bool Success, string Message)> SubscribeAsync(string email);
    }
}
