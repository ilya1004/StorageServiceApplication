using System.Linq.Expressions;
using StorageService.Domain.Primitives;

namespace StorageService.Domain.Abstractions.Data;

public interface IRepository<TEntity> where TEntity : Entity
{
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TEntity>> ListAsync(
        Expression<Func<TEntity, bool>>? filter,
        CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[]? includesProperties);

    Task<IReadOnlyList<TEntity>> PaginatedListAsync(
        int offset,
        int limit,
        Expression<Func<TEntity, bool>>? filter,
        CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[]? includesProperties);

    Task<TEntity?> FirstOrDefaultAsync(
        Expression<Func<TEntity, bool>> filter,
        CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[]? includesProperties);

    Task<TEntity?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[]? includesProperties);

    Task<int> CountAsync(
        Expression<Func<TEntity, bool>>? filter,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TResult>> ListWithProjectionAsync<TResult>(
        int offset,
        int limit,
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? filter,
        CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[]? includesProperties);

    Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>>? filter,
        CancellationToken cancellationToken = default);
}