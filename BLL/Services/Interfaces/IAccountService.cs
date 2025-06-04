using DAL.ViewModels.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IAccountService
    {
        Task<IdentityResult> RegisterUserAsync(RegisterVM model);
        Task<ApplicationUser> GetUserByEmailAsync(string email);
        Task RecordAuditLogAsync(string action, string description, Guid? userId = null);
        Task<ApplicationUser> GetUserByIdAsync(string userId);

    }
}
