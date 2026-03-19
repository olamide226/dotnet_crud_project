using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DotnetCrud.Core.Models;

namespace DotnetCrud.Core.Interfaces
{
    /// <summary>
    /// Generic repository interface for data access operations
    /// </summary>
    /// <typeparam name="T">Entity type that inherits from BaseEntity</typeparam>
    public interface IRepository<T> where T : BaseEntity
    {
        /// <summary>
        /// Get entity by ID
        /// </summary>
        Task<T?> GetByIdAsync(Guid id);

        /// <summary>
        /// Get all entities (use with caution for large datasets)
        /// </summary>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Get paginated results
        /// </summary>
        Task<(IEnumerable<T> Items, int TotalCount)> GetPaginatedAsync(
            int page, 
            int pageSize,
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            string includeProperties = "");

        /// <summary>
        /// Find entities matching a condition
        /// </summary>
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Add new entity
        /// </summary>
        Task<T> AddAsync(T entity);

        /// <summary>
        /// Add multiple entities
        /// </summary>
        Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);

        /// <summary>
        /// Update existing entity
        /// </summary>
        Task UpdateAsync(T entity);

        /// <summary>
        /// Update multiple entities
        /// </summary>
        Task UpdateRangeAsync(IEnumerable<T> entities);

        /// <summary>
        /// Delete entity (soft delete if configured)
        /// </summary>
        Task DeleteAsync(Guid id);

        /// <summary>
        /// Delete multiple entities
        /// </summary>
        Task DeleteRangeAsync(IEnumerable<Guid> ids);

        /// <summary>
        /// Hard delete - permanently remove from database
        /// </summary>
        Task HardDeleteAsync(Guid id);

        /// <summary>
        /// Check if entity exists
        /// </summary>
        Task<bool> ExistsAsync(Guid id);

        /// <summary>
        /// Get count of entities matching condition
        /// </summary>
        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);

        /// <summary>
        /// Save changes to database
        /// </summary>
        Task<int> SaveChangesAsync();
    }
}