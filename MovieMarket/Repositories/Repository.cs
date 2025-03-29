using MovieMarket.DataAccess;
using MovieMart.Repositories.IRepositories;
using System.Linq.Expressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MovieMart.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly MovieMarketDbContext _movieMarketDbContext;
        public DbSet<T> dbSet;

        /// <summary>
        /// Initializes a new instance of the Repository class with the specified DbContext.
        /// </summary>
        /// <param name="movieMartDbContext">The database context to be used.</param>
        public Repository(MovieMarketDbContext movieMartDbContext)
        {
            this._movieMarketDbContext = movieMartDbContext;
            dbSet = _movieMarketDbContext.Set<T>();
        }

        #region CRUD Operations

        #region Create operations

        /// <summary>
        /// Adds a single entity to the database.
        /// </summary>
        /// <param name="entity">The entity to add.</param>        
        public void Create(T entity)
        {
            dbSet.Add(entity);
        }

        /// <summary>
        /// Adds multiple entities to the database.
        /// </summary>
        /// <param name="entities">The list of entities to add.</param>
        public void CreateAll(List<T> entities)
        {
            dbSet.AddRange(entities);
        }

        #endregion


        #region Update operation

        /// <summary>
        /// Updates an existing entity in the database.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        public void Edit(T entity)
        {
            dbSet.Update(entity);
        }

        #endregion


        #region Delete operations

        /// <summary>
        /// Removes a single entity from the database.
        /// </summary>
        /// <param name="entity">The entity to remove.</param>
        public void Delete(T entity)
        {
            dbSet.Remove(entity);
        }

        /// <summary>
        /// Removes multiple entities from the database.
        /// </summary>
        /// <param name="entities">The list of entities to remove.</param>
        public void DeleteAll(List<T> entities)
        {
            dbSet.RemoveRange(entities);
        }

        #endregion


        #region Save changes to the database

        /// <summary>
        /// Saves all changes made to the database.
        /// </summary>    
        public void SaveDB()
        {
            _movieMarketDbContext.SaveChanges();
        }

        #endregion


        #region Read operations

        /// <summary>
        /// Retrieves entities from the database based on the provided filter, includes, and tracking options.
        /// </summary>
        /// <param name="filter">Optional filter condition.</param>
        /// <param name="includes">Optional navigation properties to include.</param>
        /// <param name="tracked">Determines whether tracking is enabled.</param>
        /// <returns>An IQueryable of the requested entities.</returns>
        public IQueryable<T> Get(Expression<Func<T, bool>>? filter = null, Expression<Func<T,
            object>>[]? includes = null, bool tracked = true)
        {
            IQueryable<T> queryableable = dbSet;

            if (filter != null)
            {
                queryableable = queryableable.Where(filter);
            }

            if (includes != null)
            {
                foreach (var item in includes)
                {
                    queryableable = queryableable.Include(item);
                }
            }

            if (!tracked)
            {
                queryableable = queryableable.AsNoTracking();
            }

            return queryableable;
        }


        /// <summary>
        /// Retrieves a single entity from the database based on the provided filter, includes, and tracking options.
        /// </summary>
        /// <param name="filter">Optional filter condition.</param>
        /// <param name="includes">Optional navigation properties to include.</param>
        /// <param name="tracked">Determines whether tracking is enabled.</param>
        /// <returns>The first matching entity or null if not found.</returns>
        public T? GetOne(Expression<Func<T, bool>>? filter = null, Expression<Func<T,
           object>>[]? includes = null, bool tracked = true)
        {
            return Get(filter, includes, tracked).FirstOrDefault();
        }

        #endregion

        #endregion
    }
}