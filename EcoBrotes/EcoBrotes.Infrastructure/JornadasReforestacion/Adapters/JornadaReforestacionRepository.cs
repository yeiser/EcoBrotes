using JornadaEntity = EcoBrotes.Domain.JornadasReforestacion.Entity.JornadaReforestacion;
using EcoBrotes.Application.Ports;
using EcoBrotes.Domain.JornadasReforestacion.Port;
using EcoBrotes.Infrastructure.Adapters;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EcoBrotes.Infrastructure.JornadasReforestacion.Adapters
{
    [Repository]
    public class JornadaReforestacionRepository(IRepository<JornadaEntity> jornadaRepository) : IJornadaReforestacionRepository
    {
        public async Task<JornadaEntity> AddAsync(JornadaEntity jornada) => await jornadaRepository.AddAsync(jornada);

        public async Task<JornadaEntity> GetByIdAsync(Guid id) => await jornadaRepository.GetOneAsync(id);

        public async Task<JornadaEntity> GetByIdAsync(Guid id, string? include) => await jornadaRepository.GetOneAsync(id, include);

        public async Task<int> GetCountAsync() => await jornadaRepository.GetCountAsync();

        public async Task<int> GetCountAsync(
            Guid? zonaId,
            IEnumerable<string>? estados,
            DateTime? fechaDesde,
            DateTime? fechaHasta)
        {
            Expression<Func<JornadaEntity, bool>>? filter = null;

            if (zonaId.HasValue)
            {
                var zonaIdValue = zonaId.Value;
                // First predicate evaluated: filter is still null, so assign it directly.
                filter = j => j.ZonaUrbanaId == zonaIdValue;
            }

            if (estados != null && estados.Any())
            {
                var estadosUpper = estados.Select(e => e.ToUpperInvariant()).ToList();
                var estadoFilter = (Expression<Func<JornadaEntity, bool>>)(j => estadosUpper.Contains(j.State.ToString().ToUpperInvariant()));
                filter = filter == null ? estadoFilter : CombineAnd(filter, estadoFilter);
            }

            if (fechaDesde.HasValue)
            {
                var fechaDesdeValue = fechaDesde.Value;
                var fechaFilter = (Expression<Func<JornadaEntity, bool>>)(j => j.ScheduledDate >= fechaDesdeValue);
                filter = filter == null ? fechaFilter : CombineAnd(filter, fechaFilter);
            }

            if (fechaHasta.HasValue)
            {
                var fechaHastaValue = fechaHasta.Value;
                var fechaFilter2 = (Expression<Func<JornadaEntity, bool>>)(j => j.ScheduledDate <= fechaHastaValue);
                filter = filter == null ? fechaFilter2 : CombineAnd(filter, fechaFilter2);
            }

            if (filter != null)
            {
                var items = await jornadaRepository.GetManyAsync(filter).ConfigureAwait(false);
                return items.Count();
            }
            return await jornadaRepository.GetCountAsync();
        }

        public async Task<IEnumerable<JornadaEntity>> GetManyAsync() => await jornadaRepository.GetManyAsync();

        public async Task<IEnumerable<JornadaEntity>> GetFilteredAsync(
            Guid? zonaId = null,
            IEnumerable<string>? estados = null,
            DateTime? fechaDesde = null,
            DateTime? fechaHasta = null,
            int page = 1,
            int pageSize = 50)
        {
            // Build filter expression dynamically
            Expression<Func<JornadaEntity, bool>>? filter = null;

            if (zonaId.HasValue)
            {
                var zonaIdValue = zonaId.Value;
                // First predicate evaluated: filter is still null, so assign it directly.
                filter = j => j.ZonaUrbanaId == zonaIdValue;
            }

            if (estados != null && estados.Any())
            {
                var estadosUpper = estados.Select(e => e.ToUpperInvariant()).ToList();
                var estadoFilter = (Expression<Func<JornadaEntity, bool>>)(j => estadosUpper.Contains(j.State.ToString().ToUpperInvariant()));
                filter = filter == null ? estadoFilter : CombineAnd(filter, estadoFilter);
            }

            if (fechaDesde.HasValue)
            {
                var fechaDesdeValue = fechaDesde.Value;
                var fechaFilter = (Expression<Func<JornadaEntity, bool>>)(j => j.ScheduledDate >= fechaDesdeValue);
                filter = filter == null ? fechaFilter : CombineAnd(filter, fechaFilter);
            }

            if (fechaHasta.HasValue)
            {
                var fechaHastaValue = fechaHasta.Value;
                var fechaFilter2 = (Expression<Func<JornadaEntity, bool>>)(j => j.ScheduledDate <= fechaHastaValue);
                filter = filter == null ? fechaFilter2 : CombineAnd(filter, fechaFilter2);
            }

            // RB-03: Order by scheduled date ascending
            Func<IQueryable<JornadaEntity>, IOrderedQueryable<JornadaEntity>>? orderBy = null;
            orderBy = query => query.OrderBy(j => j.ScheduledDate);

            // Get all matching entities
            var all = await jornadaRepository.GetManyAsync(filter, orderBy);
            var list = all.ToList();

            // Pagination in memory
            return list
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        private static Expression<Func<T, bool>> CombineAnd<T>(Expression<Func<T, bool>> a, Expression<Func<T, bool>> b)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var body = Expression.AndAlso(
                Expression.Invoke(a, parameter),
                Expression.Invoke(b, parameter)
            );
            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        public void Update(JornadaEntity jornada) => jornadaRepository.UpdateAsync(jornada);

        public async Task<IEnumerable<JornadaEntity>> GetByZonaIdAsync(Guid zonaId)
        {
            // Get all jornadas that reference this zone (including different states)
            Expression<Func<JornadaEntity, bool>> filter = j => j.ZonaUrbanaId == zonaId;
            return await jornadaRepository.GetManyAsync(filter);
        }

        public async Task<IEnumerable<JornadaEntity>> GetByEspecieArboreaIdAsync(Guid especieId)
        {
            // Get all jornadas that have tree details with this species
            // Need to include DetalleArboles to filter by species
            var allJornadas = await jornadaRepository.GetManyAsync(filter: null, orderBy: null, includeStringProperties: "DetalleArboles");
            return allJornadas.Where(j => j.DetalleArboles.Any(d => d.EspecieArboreaId == especieId));
        }
    }
}
