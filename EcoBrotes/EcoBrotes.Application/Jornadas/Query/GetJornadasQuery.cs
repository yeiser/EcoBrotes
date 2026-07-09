using MediatR;
using EcoBrotes.Domain.JornadasReforestacion.Entity;
using EcoBrotes.Domain.JornadasReforestacion.Port;
using EcoBrotes.Domain.ZonaUrbana.Port;
using EcoBrotes.Domain.EspecieArborea.Port;

namespace EcoBrotes.Application.Jornadas.Query
{
    public class GetJornadasQuery : IRequest<PaginatedResult<JornadaSummaryDto>>
    {
        public Guid? ZonaId { get; set; }
        public IEnumerable<string>? Estados { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }

    public class GetJornadasHandler(
        IJornadaReforestacionRepository jornadaRepository,
        IZonaUrbanaRepository zonaRepository) : IRequestHandler<GetJornadasQuery, PaginatedResult<JornadaSummaryDto>>
    {
        public async Task<PaginatedResult<JornadaSummaryDto>> Handle(GetJornadasQuery request, CancellationToken cancellationToken)
        {
            var totalRecords = await jornadaRepository.GetCountAsync(
                zonaId: request.ZonaId,
                estados: request.Estados,
                fechaDesde: request.FechaDesde,
                fechaHasta: request.FechaHasta);
            
            var jornadas = await jornadaRepository.GetFilteredAsync(
                zonaId: request.ZonaId,
                estados: request.Estados,
                fechaDesde: request.FechaDesde,
                fechaHasta: request.FechaHasta,
                page: request.Page,
                pageSize: request.PageSize);

            // Map to DTOs
            var result = new List<JornadaSummaryDto>();
            foreach (var jornada in jornadas)
            {
                var zona = await zonaRepository.GetByIdAsync(jornada.ZonaUrbanaId);
                var totalInscritos = jornada.TotalInscritos;
                var ocupacionPct = jornada.VolunteerCapacity > 0
                    ? (decimal)totalInscritos / jornada.VolunteerCapacity * 100
                    : 0m;

                result.Add(new JornadaSummaryDto
                {
                    Id = jornada.Id,
                    CodigoUnico = jornada.CodigoUnico,
                    Name = jornada.Name,
                    ZonaNombre = zona?.Name ?? "Desconocida",
                    ScheduledDate = jornada.ScheduledDate,
                    TreeMeta = jornada.TreeMeta,
                    VolunteerCapacity = jornada.VolunteerCapacity,
                    TotalInscritos = totalInscritos,
                    OcupacionPct = ocupacionPct,
                    Estado = jornada.State.ToString()
                });
            }

            return new PaginatedResult<JornadaSummaryDto>
            {
                Items = result,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalRecords
            };
        }
    }
}
