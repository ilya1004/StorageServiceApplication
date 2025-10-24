using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using StorageService.Domain.Abstractions.Data;
using StorageService.Domain.Primitives;
using StorageService.Infrastructure.Data;
using StorageService.Infrastructure.Extensions;

namespace StorageService.Infrastructure.Repositories;

public class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : Entity
{
    private readonly DbSet<TEntity> _entities;

    public BaseRepository(ApplicationDbContext context)
    {
        _entities = context.Set<TEntity>();
    }

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await _entities.AddAsync(entity, cancellationToken);
    }

    public Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _entities.Update(entity);

        return Task.CompletedTask;
    }

    public Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _entities.Remove(entity);

        return Task.CompletedTask;
    }

    public async Task<IReadOnlyList<TEntity>> ListAsync(
        Expression<Func<TEntity, bool>>? filter,
        CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[]? includesProperties)
    {
        return await _entities
            .AddFilter(filter)
            .AddIncludes(includesProperties)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TEntity>> PaginatedListAsync(
        int offset,
        int limit,
        Expression<Func<TEntity, bool>>? filter,
        CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[]? includesProperties)
    {
        return await _entities
            .AddFilter(filter)
            .AddIncludes(includesProperties)
            .OrderBy(x => x.Id)
            .Skip(offset)
            .Take(limit)
            .ToListAsync(cancellationToken);

    }

    public async Task<TEntity?> FirstOrDefaultAsync(
        Expression<Func<TEntity, bool>> filter,
        CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[]? includesProperties)
    {
        return await _entities.FirstOrDefaultAsync(filter, cancellationToken);
    }

    public async Task<TEntity?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[]? includesProperties)
    {
        return await _entities.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<int> CountAsync(
        Expression<Func<TEntity, bool>>? filter,
        CancellationToken cancellationToken = default)
    {
        return filter is not null
            ? await _entities.CountAsync(filter, cancellationToken)
            : await _entities.CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TResult>> ListWithProjectionAsync<TResult>(
        int offset,
        int limit,
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? filter = null,
        CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[]? includesProperties)
    {
        return await _entities
            .AsNoTracking()
            .AddFilter(filter)
            .AddIncludes(includesProperties)
            .OrderBy(x => x.Id)
            .Skip(offset)
            .Take(limit)
            .Select(selector)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>>? filter,
        CancellationToken cancellationToken = default)
    {
        return filter is not null
            ? await _entities.AnyAsync(filter, cancellationToken)
            : await _entities.AnyAsync(cancellationToken);
    }


    // public Task<IReadOnlyList<TEntity>> GetByFilterAsync(
    //     ISpecification<TEntity> specification,
    //     CancellationToken cancellationToken = default)
    // {
    //     throw new NotImplementedException();
    // }
}