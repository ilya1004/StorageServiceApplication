using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using StorageService.Domain.Primitives;

namespace StorageService.Infrastructure.Extensions;

public static class RepositoryExtensions
{
    public static IQueryable<TEntity> AddIncludes<TEntity>(
        this IQueryable<TEntity> query,
        Expression<Func<TEntity, object>>[]? includesProperties) where TEntity : Entity
    {
        if (includesProperties is not null)
        {
            foreach (var includeProperty in includesProperties)
            {
                query = query.Include(includeProperty);
            }
        }

        return query;
    }

    public static IQueryable<TEntity> AddFilter<TEntity>(
        this IQueryable<TEntity> query,
        Expression<Func<TEntity, bool>>? filter) where TEntity : Entity
    {
        if (filter is not null)
        {
            query = query.Where(filter);
        }

        return query;
    }
}