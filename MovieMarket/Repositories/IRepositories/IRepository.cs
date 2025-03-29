using System.Linq.Expressions;

namespace MovieMart.Repositories.IRepositories
{
    public interface IRepository<T>
    {


        #region CRUD Operations

        #region Create Operations

        /// <summary>
        /// Adds a single entity to the database.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        public void Create(T entity);

        /// <summary>
        /// Adds multiple entities to the database.
        /// </summary>
        /// <param name="entities">The list of entities to add.</param>
        public void CreateAll(List<T> entities);

        #endregion


        #region Update Operation

        /// <summary>
        /// Updates an existing entity in the database.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        public void Edit(T entity);

        #endregion


        #region Delete Operations 

        /// <summary>
        /// Removes a single entity from the database.
        /// </summary>
        /// <param name="entity">The entity to remove.</param>
        public void Delete(T entity);

        /// <summary>
        /// Removes multiple entities from the database.
        /// </summary>
        /// <param name="entities">The list of entities to remove.</param>
        public void DeleteAll(List<T> entities);

        #endregion


        #region Save Changes to Database

        /// <summary>
        /// Saves all changes made to the database.
        /// </summary>
        public void SaveDB();

        #endregion

        #region Read Operations

        /// <summary>
        /// Retrieves entities from the database based on the provided filter, includes, and tracking options.
        /// </summary>
        /// <param name="filter">Optional filter condition.</param>
        /// <param name="includes">Optional navigation properties to include.</param>
        /// <param name="tracked">Determines whether tracking is enabled.</param>
        /// <returns>An IQueryable of the requested entities.</returns>
        public IQueryable<T> Get(Expression<Func<T, bool>>? filter = null, Expression<Func<T,
            object>>[]? includes = null, bool tracked = true);


        /// <summary>
        /// Retrieves a single entity from the database based on the provided filter, includes, and tracking options.
        /// </summary>
        /// <param name="filter">Optional filter condition.</param>
        /// <param name="includes">Optional navigation properties to include.</param>
        /// <param name="tracked">Determines whether tracking is enabled.</param>
        /// <returns>The first matching entity or null if not found.</returns>
        public T? GetOne(Expression<Func<T, bool>>? filter = null, Expression<Func<T,
           object>>[]? includes = null, bool tracked = true);

        #endregion

        #endregion
    }
}
