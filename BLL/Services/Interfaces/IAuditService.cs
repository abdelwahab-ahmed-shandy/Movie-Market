using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IAuditService
    {
        string GetCurrentUserName();
        //Task RecordAuditAsync(Guid userId, string action, string description, string entityId = null); string GetCurrentUserName();
        Task RecordAuditAsync(Guid userId, string action, string description, string entityId = null, string performedBy = null);
    }
}
