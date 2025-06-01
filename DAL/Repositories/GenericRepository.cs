
using DAL.Repositories.IRepositories;

namespace DAL.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseModel
    {
        private ApplicationdbContext _context;
        private DbSet<T> _dbSet;
        public GenericRepository(ApplicationdbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }



        #region CRUD operations






        #region Soft Delete
        public async Task SoftDeleteAsync(int id, string deletedBy)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                entity.IsDeleted = true;
                entity.DeletedBy = deletedBy;
                entity.DeletedDateUtc = DateTime.UtcNow;

                _dbSet.Update(entity);
                await _context.SaveChangesAsync();
            }
        }
        #endregion

        #endregion

    }
}
