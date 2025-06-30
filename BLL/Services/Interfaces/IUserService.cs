using DAL.ViewModels.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<UserIndexVM>> GetAllUsersAsync();
        Task<List<UserIndexVM>> GetAdminsAsync();
        Task<List<UserIndexVM>> GetSuperAdminsAsync();
        Task<List<UserIndexVM>> GetCustomersAsync();

        Task<UserDetailsVM> GetUserDetailsAsync(Guid id);
        Task BlockUserAsync(Guid userId, string blockReason, string blockedBy);
        Task UnblockUserAsync(Guid userId, string unblockedBy);

        Task ChangeUserRoleAsync(ChangeUserRoleVM model, string changedBy);
        Task<ChangeUserRoleVM> GetUserRoleInfoAsync(Guid userId);
    }
}
