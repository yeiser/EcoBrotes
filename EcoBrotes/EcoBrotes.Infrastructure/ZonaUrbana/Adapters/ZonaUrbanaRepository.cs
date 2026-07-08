using ZonaUrbanaEntity = EcoBrotes.Domain.ZonaUrbana.Entity.ZonaUrbanaEntity;
using EcoBrotes.Application.Ports;
using EcoBrotes.Domain.Common;
using EcoBrotes.Domain.ZonaUrbana.Entity;
using EcoBrotes.Domain.ZonaUrbana.Port;
using EcoBrotes.Infrastructure.Adapters;
using EcoBrotes.Infrastructure.DataSource;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EcoBrotes.Infrastructure.ZonaUrbana.Adapters
{
    [Repository]
    public class ZonaUrbanaRepository(
        IRepository<ZonaUrbanaEntity> zonaRepository,
        DataContext context) : IZonaUrbanaRepository
    {
        // Filter by active state for all queries (Soft Delete pattern)
        private static readonly Expression<Func<ZonaUrbanaEntity, bool>> ActiveFilter = z => z.State == ZonaState.Activa;

        public async Task<ZonaUrbanaEntity> AddAsync(ZonaUrbanaEntity zona) => await zonaRepository.AddAsync(zona);

        public async Task<ZonaUrbanaEntity> GetByIdAsync(Guid id)
        {
            // Try without filter first (for handlers that need to find deleted entities)
            var zona = await zonaRepository.GetOneAsync(id);
            return zona;
        }

        public async Task<ZonaUrbanaEntity> GetByIdAsync(Guid id, string? include) => await zonaRepository.GetOneAsync(id, include);

        public async Task<int> GetCountAsync()
        {
            var all = await zonaRepository.GetManyAsync(ActiveFilter);
            return all.Count();
        }

        public async Task<IEnumerable<ZonaUrbanaEntity>> GetManyAsync() => await zonaRepository.GetManyAsync(ActiveFilter);

        public async Task<ZonaUrbanaEntity> GetByNameAsync(string name)
        {
            // Efficient query with filter - database does the work
            return await context.Set<ZonaUrbanaEntity>()
                .Where(ActiveFilter)
                .FirstOrDefaultAsync(z => z.Name.ToLower() == name.ToLower());
        }

        public async Task DeactivateAsync(Guid id)
        {
            var zona = await zonaRepository.GetOneAsync(id);
            zona.ValidateNull($"La zona urbana con id {id} no existe.");
            zona.Deactivate();
        }
    }
}
