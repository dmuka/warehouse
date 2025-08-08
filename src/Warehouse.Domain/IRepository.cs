using System.Linq.Expressions;
using Warehouse.Core;

namespace Warehouse.Domain
{
    /// <summary>
    /// Contains base repository methods. If you register the multiple DbContexts, it will use the last one.
    /// </summary>
    public interface IRepository<TEntity> where TEntity : Entity
    {
        /// <summary>
        /// Gets <see cref="IQueryable{T}"/> of the entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns>Returns <see cref="IQueryable{T}"/>.</returns>
        IQueryable<TEntity> GetQueryable();

        /// <summary>
        /// Materialize asynchronously <see cref="IQueryable{TEntity}"/> to <see cref="IList{TEntity}"/>.
        /// </summary>
        /// <param name="query"><see cref="IQueryable{TEntity}"/></param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <returns>
        /// <see><cref>Task{IList{T}}</cref></see>
        /// </returns>
        Task<IList<TEntity>> QueryableToListAsync(IQueryable<TEntity> query, CancellationToken cancellationToken);

        #region Get list

        /// <summary>
        /// Asynchronously retrieves a list of all entities of type <typeparamref name="TEntity"/> from the data source.
        /// </summary>
        /// <param name="include">An optional function to include related entities in the query.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation, with a result of <see cref="IList{TEntity}"/> containing all entities.</returns>
        Task<IList<TEntity>> GetListAsync(CancellationToken cancellationToken = default, Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null);

        #endregion

        #region Get by id
        
        /// <summary>
        /// This method takes <paramref name="id"/> which is the primary key value of the entity and returns the entity
        /// if found otherwise <see langword="null"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="id">The primary key value of the entity.</param>
        /// <param name="cancellationToken"> A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>Returns <see cref="Task{TResult}"/>.</returns>
        Task<TEntity?> GetByIdAsync(TypedId id, CancellationToken cancellationToken = default);

        #endregion

        #region Get entity by condition
        
        /// <summary>
        /// This method takes <see cref="Expression{Func}"/> as parameter and returns <typeparamref name="TEntity"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="condition">The condition on which entity will be returned.</param>
        /// <param name="cancellationToken"> A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>Returns <typeparamref name="TEntity"/>.</returns>
        Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> condition, CancellationToken cancellationToken = default);

        #endregion

        #region Exists

        /// <summary>
        /// This method takes primary key value of the entity whose existence be determined
        /// and returns <see cref="Task"/> of <see cref="bool"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="id">The primary key value of the entity whose the existence will checked.</param>
        /// <param name="cancellationToken"> A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>Returns <see cref="bool"/>.</returns>
        Task<bool> ExistsByIdAsync(TypedId id, CancellationToken cancellationToken = default);

        #endregion

        #region Raw SQL

        /// <summary>
        /// This method takes <paramref name="sql"/> string as parameter and returns the result of the provided sql.
        /// </summary>
        /// <typeparam name="TEntity">The <see langword="type"/> to which the result will be mapped.</typeparam>
        /// <param name="sql">The sql query string.</param>
        /// <param name="parameters">Sql query parameters collection</param>
        /// <param name="cancellationToken"> A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>Returns <see cref="Task{TResult}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="sql"/> is <see langword="null"/>.</exception>
        Task<IList<TEntity>> GetFromRawSqlAsync(string sql, List<object>? parameters, CancellationToken cancellationToken = default);

        #endregion

        /// <summary>
        /// Reset the DbContext state by removing all the tracked and attached entities.
        /// </summary>
        void ClearChangeTracker();

        #region Add
        /// <summary>
        /// This method takes an <typeparamref name="TEntity"/> object, mark the object as <see cref="EntityState.Added"/> to the <see cref="ChangeTracker"/> of the <see cref="DbContext"/>.
        /// <para>
        /// Call <see cref="SaveChangesAsync(CancellationToken)"/> to persist the changes to the database.
        /// </para>
        /// </summary>
        /// <typeparam name="TEntity">The type of the <paramref name="entity"/> to be added.</typeparam>
        /// <param name="entity">The <typeparamref name="TEntity"/> object to be inserted to the database on <see cref="SaveChangesAsync(CancellationToken)"/>.</param>
        void Add(TEntity entity);

        #endregion

        #region Update
        
        /// <summary>
        /// This method takes an <typeparamref name="TEntity"/> object, mark the object as <see cref="EntityState.Modified"/> to the <see cref="ChangeTracker"/> of the <see cref="DbContext"/>.
        /// <para>
        /// Call <see cref="SaveChangesAsync(CancellationToken)"/> to persist the changes to the database.
        /// </para>
        /// </summary>
        /// <typeparam name="TEntity">The type of the <paramref name="entity"/> to be marked as modified.</typeparam>
        /// <param name="entity">The <typeparamref name="TEntity"/> object to be updated to the database on <see cref="SaveChangesAsync(CancellationToken)"/>.</param>
        void Update(TEntity entity);

        #endregion

        #region Delete

        /// <summary>
        /// This method takes an <typeparamref name="TEntity"/>, mark the object as <see cref="EntityState.Deleted"/> to the <see cref="ChangeTracker"/> of the <see cref="DbContext"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <paramref name="entityId"/> to be marked as deleted.</typeparam>
        /// <param name="entityId">The primary key value of the entity.</param>
        Task Delete(TypedId entityId);
        
        #endregion

        #region Where

        /// <summary>
        /// This method takes <see> <cref>Expression{Func{T, bool}}</cref> </see> predicate and return result of filtering by this predicate.
        /// </summary>
        /// <param name="condition">The condition based on which filter of items will be done.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <typeparam name="TEntity">The type of the items to be returned.</typeparam>
        /// <returns>A <see cref="Task"/> that represents the asynchronous save operation. The task result contains the list of items that represent result of filtering.</returns>
        public Task<IList<TEntity>> GetEntitiesByCondition(Expression<Func<TEntity, bool>> condition,
            CancellationToken cancellationToken = default);

        #endregion
    }
}