
namespace DAL.Repositories
{
    public class ApplicationUserRepository : IApplicationUserRepository
    {
        private readonly ApplicationdbContext _context;

        public ApplicationUserRepository(ApplicationdbContext context)
        {
            _context = context;
        }

        public async Task<ApplicationUser> GetByIdAsync(Guid Id)
        {
            return await _context.Users.AsTracking().FirstOrDefaultAsync(u => u.Id == Id);
        }

        public async Task<ApplicationUser?> GetOneAsync(Expression<Func<ApplicationUser, bool>> predicate)
        {
            return await _context.Users.AsTracking().FirstOrDefaultAsync(predicate);
        }

        public async Task Update(ApplicationUser user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public IQueryable<ApplicationUser> GetAll()
        {
            return _context.Users.AsNoTracking();
        }

        public async Task<List<ApplicationUser>> GetAllAsync()
        {
            return await _context.Users.AsTracking().ToListAsync();
        }

        public async Task Create(ApplicationUser user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public void Delete(ApplicationUser user)
        {
            _context.Users.Remove(user);
        }

        public async Task<int> GetTotalUsersAsync()
        {
            return await _context.Users.CountAsync();
        }

    }
}
