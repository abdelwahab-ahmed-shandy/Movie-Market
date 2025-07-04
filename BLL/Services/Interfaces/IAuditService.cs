
namespace BLL.Services.Interfaces
{
    public interface IAuditService
    {
        string GetCurrentUserName();
        Task RecordAuditAsync(Guid userId, string action, string description, string entityId = null, string performedBy = null);
    }
}
