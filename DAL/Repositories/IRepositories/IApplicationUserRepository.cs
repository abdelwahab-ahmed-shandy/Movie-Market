
namespace DAL.Repositories.IRepositories
{
    public interface IApplicationUserRepository
    {
        Task<ApplicationUser> GetByIdAsync(Guid Id);
        Task<ApplicationUser?> GetOneAsync(Expression<Func<ApplicationUser, bool>> predicate);
        Task<List<ApplicationUser>> GetAllAsync();
        IQueryable<ApplicationUser> GetAll();
        Task Create(ApplicationUser user);
        Task Update(ApplicationUser user);
        void Delete(ApplicationUser user);

        Task<int> GetTotalUsersAsync();
    }
}
