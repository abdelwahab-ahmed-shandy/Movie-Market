using BLL.Services.Interfaces;
using DAL.Context;
using Microsoft.Extensions.Logging;

namespace BLL.Services.Implementations
{
    public class AuditService : IAuditService
    {
        private readonly ApplicationdbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AuditService> _logger;

        public AuditService(
            ApplicationdbContext context,
            IHttpContextAccessor httpContextAccessor,
            ILogger<AuditService> logger)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public string GetCurrentUserName()
        {
            return _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
        }

        public async Task RecordAuditAsync(Guid userId, string action, string description, string entityId = null, string performedBy = null)
        {
            try
            {
                var currentUser = performedBy ?? GetCurrentUserName();
                var auditRecord = new AuditRecord
                {
                    UserId = userId,
                    Action = action,
                    Description = description,
                    EntityId = entityId,
                    LastAction = action,
                    CreatedBy = currentUser
                };

                _context.AuditRecords.Add(auditRecord);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recording audit log");
                throw;
            }
        }

    }

}
