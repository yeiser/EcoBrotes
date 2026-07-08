using EcoBrotes.Domain.Common;
using System.Linq.Expressions;

namespace EcoBrotes.Application.Ports
{
    public interface IRepository<T> where T : DomainEntity
    {
        Task<T> GetByIdAsync(Guid id);
        Task<T> GetOneAsync(Guid id, string? includeStringProperties = default);
        Task<IEnumerable<T>> GetManyAsync(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            string includeStringProperties = "",
            bool isTracking = false);
        Task<T> AddAsync(T entity);
        void UpdateAsync(T entity);
        void DeleteAsync(T entity);
        Task<int> GetCountAsync();
    }
}
