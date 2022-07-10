using System.Linq.Expressions;
using TodoApplication.Domain.Common.Models;

namespace TodoApplication.Domain.Common.Repository;

public interface IGenericRepository<TEntity, in TKey> where TEntity : EntityBase<TKey>
{
    Task<TEntity?> GetAsync(TKey id);
    
    Task<TEntity?> GetAsyncIncluding(TKey id, params Expression<Func<TEntity, object>>[] includeProperties);

    Task<List<TEntity>> GetAllAsync();
    
    Task<List<TEntity>> GetAllAsyncIncluding(params Expression<Func<TEntity, object>>[] includeProperties);

    Task<Page<TEntity>> GetAllPagedAsync(int pageIndex, int pageSize);
    
    Task<Page<TEntity>> GetAllPagedAsyncIncluding(int pageIndex, int pageSize, params Expression<Func<TEntity, object>>[] includeProperties);

    Task<List<TEntity>> FilterAsync(Expression<Func<TEntity, bool>> filter);
    
    Task<List<TEntity>> FilterAsyncIncluding(Expression<Func<TEntity, bool>> filter, params Expression<Func<TEntity, object>>[] includeProperties);

    Task<Page<TEntity>> FilterPagedAsync(Expression<Func<TEntity, bool>> filter, int pageIndex, int pageSize);
    
    Task<Page<TEntity>> FilterPagedAsyncIncluding(Expression<Func<TEntity, bool>> filter, int pageIndex, int pageSize, params Expression<Func<TEntity, object>>[] includeProperties);

    Task<TEntity> AddAsync(TEntity entity);

    Task<List<TEntity>> AddMultipleAsync(TEntity[] entities);

    Task<TEntity> UpdateAsync(TEntity entity);

    Task<List<TEntity>> UpdateMultipleAsync(TEntity[] entities);

    Task DeleteAsync(TKey id);

    Task DeleteMultipleAsync(TKey[] ids);
}