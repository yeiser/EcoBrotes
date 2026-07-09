using EcoBrotes.Domain.JornadasReforestacion.Entity;

namespace EcoBrotes.Domain.JornadasReforestacion.Port
{
    public interface IJornadaReforestacionRepository
    {
        Task<JornadaReforestacion> AddAsync(JornadaReforestacion jornada);
        Task<JornadaReforestacion> GetByIdAsync(Guid id);
        Task<JornadaReforestacion> GetByIdAsync(Guid id, string? include);
        Task<int> GetCountAsync();
        Task<int> GetCountAsync(
            Guid? zonaId,
            IEnumerable<string>? estados,
            DateTime? fechaDesde,
            DateTime? fechaHasta);
        Task<IEnumerable<JornadaReforestacion>> GetManyAsync();
        Task<IEnumerable<JornadaReforestacion>> GetFilteredAsync(
            Guid? zonaId = null,
            IEnumerable<string>? estados = null,
            DateTime? fechaDesde = null,
            DateTime? fechaHasta = null,
            int page = 1,
            int pageSize = 50);
        Task<IEnumerable<JornadaReforestacion>> GetByZonaIdAsync(Guid zonaId);
        Task<IEnumerable<JornadaReforestacion>> GetByEspecieArboreaIdAsync(Guid especieId);
        void Update(JornadaReforestacion jornada);
    }
}
