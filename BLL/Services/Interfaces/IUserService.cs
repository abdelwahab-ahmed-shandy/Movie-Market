
namespace BLL.Services.Interfaces
{
    public interface IUserService
    {
        Task<PaginatedList<UserIndexVM>> GetAllUsersAsync(int pageIndex = 1, string search = null);
            
        Task<PaginatedList<UserIndexVM>> GetAdminsAsync(int pageIndex = 1, int pageSize = 10 , string search = null);

        Task<PaginatedList<UserIndexVM>> GetSuperAdminsAsync(int pageIndex = 1, int pageSize = 10, string search = null);
        
        Task<PaginatedList<UserIndexVM>> GetCustomersAsync(int pageIndex = 1, int pageSize = 10, string search = null);

        Task<UserDetailsVM> GetUserDetailsAsync(Guid id);
        Task BlockUserAsync(Guid userId, string blockReason);
        Task UnblockUserAsync(Guid userId);


        Task ChangeUserRoleAsync(ChangeUserRoleVM model);
        Task<ChangeUserRoleVM> GetUserRoleInfoAsync(Guid userId);
    }
}
