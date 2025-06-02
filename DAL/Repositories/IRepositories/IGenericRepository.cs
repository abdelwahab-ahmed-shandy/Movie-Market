
namespace DAL.Repositories.IRepositories
{
    public interface IGenericRepository<T> where T : BaseModel
    {

        #region CRUD operations


        #region Get By Id 

        Task<T?> GetByIdAsync(int id);

        #endregion

        #region Get One
        Task<T?> GetOneAsync(Expression<Func<T, bool>> predicate);
        #endregion

        #region Get All
        IQueryable<T> GetAll();
        #endregion

        #region Get 
        IQueryable<T> Get(Expression<Func<T, bool>> expression);
        #endregion

        #region Add
        Task<T> Add(T entity);
        #endregion

        #region Update
        Task<T> Update(T entity);
        #endregion

        #region Soft Delete 

        Task Delete(int id);
        Task SoftDeleteAsync(int id);
        Task RestoreAsync(int id);
        #endregion

        #endregion

    }
}
