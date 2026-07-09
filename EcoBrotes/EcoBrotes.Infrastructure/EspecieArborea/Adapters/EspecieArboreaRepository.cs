using EspecieArboreaEntity = EcoBrotes.Domain.EspecieArborea.Entity.EspecieArboreaEntity;
using EcoBrotes.Application.Ports;
using EcoBrotes.Domain.Common;
using EcoBrotes.Domain.EspecieArborea.Entity;
using EcoBrotes.Domain.EspecieArborea.Port;
using EcoBrotes.Infrastructure.Adapters;
using EcoBrotes.Infrastructure.DataSource;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EcoBrotes.Infrastructure.EspecieArborea.Adapters
{
    [Repository]
    public class EspecieArboreaRepository(
        IRepository<EspecieArboreaEntity> especieRepository,
        DataContext context) : IEspecieArboreaRepository
    {
        // Filter by active state for all queries (Soft Delete pattern)
        private static readonly Expression<Func<EspecieArboreaEntity, bool>> ActiveFilter = e => e.State == EspecieState.Activa;

        public async Task<EspecieArboreaEntity> AddAsync(EspecieArboreaEntity especie) => await especieRepository.AddAsync(especie);

        public async Task<EspecieArboreaEntity> GetByIdAsync(Guid id)
        {
            // Try without filter first (for handlers that need to find deleted entities)
            var especie = await especieRepository.GetOneAsync(id);
            return especie;
        }

        public async Task<EspecieArboreaEntity> GetByIdAsync(Guid id, string? include) => await especieRepository.GetOneAsync(id, include);

        public async Task<int> GetCountAsync()
        {
            var all = await especieRepository.GetManyAsync(ActiveFilter);
            return all.Count();
        }

        public async Task<IEnumerable<EspecieArboreaEntity>> GetManyAsync() => await especieRepository.GetManyAsync(ActiveFilter);

        public async Task<EspecieArboreaEntity?> GetByNameAsync(string name)
        {
            // Efficient query with filter - database does the work
            return await context.Set<EspecieArboreaEntity>()
                .Where(ActiveFilter)
                .FirstOrDefaultAsync(e => e.Name.ToLower() == name.ToLower());
        }

        public async Task DeactivateAsync(Guid id)
        {
            var especie = await especieRepository.GetOneAsync(id);
            especie.ValidateNull($"La especie con id {id} no existe.");
            especie.Deactivate();
        }
    }
}
