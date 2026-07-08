using EcoBrotes.Application.Ports;
using EcoBrotes.Domain.Common;
using EcoBrotes.Infrastructure.DataSource;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EcoBrotes.Infrastructure.Adapters;

public class GenericRepository<T> : IRepository<T> where T : DomainEntity
{
    readonly DataContext Context;
    readonly DbSet<T> _dataset;
    readonly char[] separator = [','];

    public GenericRepository(DataContext context)
    {
        Context = context;
        _dataset = Context.Set<T>();
    }

    public async Task<IEnumerable<T>> GetManyAsync(
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        string includeStringProperties = "", bool isTracking = false)
    {
        IQueryable<T> query = Context.Set<T>();

        if (filter != null)
        {
            query = query.Where(filter);
        }

        if (!string.IsNullOrEmpty(includeStringProperties))
        {
            foreach (var includeProperty in includeStringProperties.Split
                (separator, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }
        }

        if (orderBy != null)
        {
            return await orderBy(query).ToListAsync().ConfigureAwait(false);
        }

        return !isTracking ? await query.AsNoTracking().ToListAsync() : await query.ToListAsync();
    }

    public async Task<T> AddAsync(T entity)
    {
        _ = entity ?? throw new ArgumentNullException(nameof(entity), "Entity can not be null");
        await _dataset.AddAsync(entity);
        return entity;
    }

    public void DeleteAsync(T entity)
    {
        _ = entity ?? throw new ArgumentNullException(nameof(entity), "Entity can not be null");
        _dataset.Remove(entity);
    }

    public async Task<T> GetOneAsync(Guid id, string? includeStringProperties = default)
    {
        var query = _dataset.AsQueryable();

        if (!string.IsNullOrEmpty(includeStringProperties))
        {
            foreach (var includeProperty in includeStringProperties.Split(separator, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }
        }

        return await query.FirstOrDefaultAsync(entity => entity.Id == id) ?? default!;
    }

    public async Task<T> GetByIdAsync(Guid id)
    {
        return await _dataset.FindAsync(id) ?? default!;
    }

    public void UpdateAsync(T entity)
    {
        _dataset.Update(entity);
    }

    public Task<int> GetCountAsync()
    {
        return _dataset.CountAsync();
    }

}
