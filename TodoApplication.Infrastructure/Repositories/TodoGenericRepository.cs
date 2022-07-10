using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TodoApplication.Domain.Common.Models;
using TodoApplication.Domain.Common.Repository;
using TodoApplication.Infrastructure.DbContexts.Todo;

namespace TodoApplication.Infrastructure.Repositories;

public class TodoGenericRepository<TEntity, TKey> : IGenericRepository<TEntity, TKey> where TEntity : EntityBase<TKey>
{
    public long Count => _dbSet.Count();

    private readonly DbSet<TEntity> _dbSet;

    private readonly TodoDbContext _dbContext;

    protected TodoGenericRepository(TodoDbContext dbContext)
    {
        _dbContext = dbContext;

        _dbSet = dbContext.Set<TEntity>();
    }

    public async Task<TEntity?> GetAsync(TKey id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<TEntity?> GetAsyncIncluding(TKey id, params Expression<Func<TEntity, object>>[] includeProperties)
    {
        return await GetAllIncluding(includeProperties)
            .Where(x => x.Id!.Equals(id))
            .FirstOrDefaultAsync();
    }

    public async Task<List<TEntity>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<List<TEntity>> GetAllAsyncIncluding(params Expression<Func<TEntity, object>>[] includeProperties)
    {
        return await GetAllIncluding(includeProperties).ToListAsync();
    }

    public async Task<Page<TEntity>> GetAllPagedAsync(int pageIndex, int pageSize)
    {
        var items = await _dbSet
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new Page<TEntity>(items, Count, pageIndex, pageSize);
    }

    public async Task<Page<TEntity>> GetAllPagedAsyncIncluding(int pageIndex, int pageSize, params Expression<Func<TEntity, object>>[] includeProperties)
    {
        var items = await GetAllIncluding(includeProperties)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return new Page<TEntity>(items, Count, pageIndex, pageSize);
    }

    public async Task<List<TEntity>> FilterAsync(Expression<Func<TEntity, bool>> filter)
    {
        return await _dbSet
            .Where(filter)
            .ToListAsync();
    }

    public async Task<List<TEntity>> FilterAsyncIncluding(Expression<Func<TEntity, bool>> filter, params Expression<Func<TEntity, object>>[] includeProperties)
    {
        return await GetAllIncluding(includeProperties)
            .Where(filter)
            .ToListAsync();
    }

    public async Task<Page<TEntity>> FilterPagedAsync(Expression<Func<TEntity, bool>> filter, int pageIndex, int pageSize)
    {
        var items = await _dbSet
            .Where(filter)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new Page<TEntity>(items, Count, pageIndex, pageSize);
    }

    public async Task<Page<TEntity>> FilterPagedAsyncIncluding(Expression<Func<TEntity, bool>> filter, int pageIndex, int pageSize, params Expression<Func<TEntity, object>>[] includeProperties)
    {
        var items = await GetAllIncluding(includeProperties)
            .Where(filter)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new Page<TEntity>(items, Count, pageIndex, pageSize);
    }

    public async Task<TEntity> AddAsync(TEntity entity)
    {
        _dbSet.Add(entity);

        await SaveChangesAsync();

        return entity;
    }

    public async Task<List<TEntity>> AddMultipleAsync(TEntity[] entities)
    {
        await _dbSet.AddRangeAsync(entities);

        await SaveChangesAsync();

        return entities.ToList();
    }

    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
        _dbSet.Update(entity);

        await SaveChangesAsync();

        return entity;
    }

    public async Task<List<TEntity>> UpdateMultipleAsync(TEntity[] entities)
    {
        _dbSet.UpdateRange(entities);

        await SaveChangesAsync();

        return entities.ToList();
    }

    public async Task DeleteAsync(TKey id)
    {
        var entity = await _dbSet.FindAsync(id);

        if (entity != null) _dbSet.Remove(entity);

        await SaveChangesAsync();
    }

    public async Task DeleteMultipleAsync(TKey[] ids)
    {
        var entities = await _dbSet
            .Where(x => ids.Contains(x.Id))
            .ToListAsync();

        _dbSet.RemoveRange(entities);

        await SaveChangesAsync();
    }

    private async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }

    private IQueryable<TEntity> GetAllIncluding(params Expression<Func<TEntity, object>>[] includeProperties)
    {
        return includeProperties.Aggregate(
            _dbSet.AsTracking(),
            (entity, includeProperty) => entity.Include(includeProperty)
        );
    }
}