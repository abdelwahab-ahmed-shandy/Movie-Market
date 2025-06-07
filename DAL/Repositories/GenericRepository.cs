
using DAL.Repositories.IRepositories;

namespace DAL.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationdbContext _context;
        private readonly DbSet<T> _dbSet;
        public GenericRepository(ApplicationdbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _dbSet = _context.Set<T>();
        }

        #region CRUD operations


        #region Get By Id 

        public async Task<T> GetByIdAsync(Guid Id)
        {
            return await _dbSet.AsTracking().Where(e => e.Id == Id).FirstOrDefaultAsync();
        }
        #endregion


        #region Get One 

        public async Task<T?> GetOneAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AsTracking().FirstOrDefaultAsync(predicate);
        }

        #endregion


        #region Get All 

        public IQueryable<T> GetAll()
        {
            return _dbSet.Where(e => !e.IsDeleted);
        }

        #endregion


        #region Get 

        public IQueryable<T> Get(Expression<Func<T, bool>> expression)
        {
            var res = GetAll().Where(expression);
            return res;
        }

        #endregion


        #region Get All With Deleted

        public IQueryable<T> GetAllWithDeleted()
        {
            return _dbSet;
        }

        #endregion


        #region Add 

        public async Task<T> Add(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        #endregion


        #region Update 

        public async Task<T> Update(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        #endregion


        #region Soft Delete

        #region private methods
        private string? GetCurrentUserName()
            => _httpContextAccessor.HttpContext?.User?.Identity?.Name;

        private async Task<T?> FindByIdAsync(Guid id)
            => await _dbSet.FindAsync(id);

        private void MarkAsDeleted(T entity, string deletedBy)
        {
            entity.IsDeleted = false;
            entity.CurrentState = CurrentState.SoftDelete;
            entity.DeletedBy = deletedBy;
            entity.DeletedDateUtc = DateTime.UtcNow;
        }

        private void MarkAsRestored(T entity, string restoredBy)
        {
            entity.IsDeleted = false;
            entity.CurrentState = CurrentState.RestoreDeleted;
            entity.RestoredBy = restoredBy;
            entity.RestoredDateUtc = DateTime.UtcNow;
        }

        #endregion


        public async Task SoftDeleteAsync(Guid id)
        {
            var entity = await FindByIdAsync(id);
            if (entity != null && !entity.IsDeleted)
            {
                MarkAsDeleted(entity, GetCurrentUserName() ?? "System");
                _dbSet.Update(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RestoreAsync(Guid id)
        {
            var entity = await FindByIdAsync(id);
            if (entity != null && entity.IsDeleted)
            {
                MarkAsRestored(entity, GetCurrentUserName() ?? "System");
                _dbSet.Update(entity);
                await _context.SaveChangesAsync();
            }
        }


        #endregion


        #region Delete 

        public async Task DeleteInDB(Guid id)
        {
            var entity = await FindByIdAsync(id);

            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }

        }

        #endregion

        #endregion

    }
}