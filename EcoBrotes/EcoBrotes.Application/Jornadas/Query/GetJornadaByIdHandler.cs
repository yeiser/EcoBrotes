using EcoBrotes.Application.Jornadas.Query;
using EcoBrotes.Domain.EspecieArborea.Port;
using EcoBrotes.Domain.JornadasReforestacion.Entity;
using EcoBrotes.Domain.JornadasReforestacion.Port;
using EcoBrotes.Domain.ZonaUrbana.Port;
using MediatR;

namespace EcoBrotes.Application.Jornadas.Query
{
    public class GetJornadaByIdHandler(
        IJornadaReforestacionRepository jornadaRepository,
        IZonaUrbanaRepository zonaRepository,
        IEspecieArboreaRepository especieRepository) : IRequestHandler<GetJornadaByIdQuery, JornadaDetailDto>
    {
        public async Task<JornadaDetailDto> Handle(GetJornadaByIdQuery request, CancellationToken cancellationToken)
        {
            var jornada = await jornadaRepository.GetByIdAsync(request.Id);
            if (jornada == null)
            {
                throw new EcoBrotes.Domain.Exceptions.CoreBusinessException($"La jornada con id {request.Id} no existe.");
            }

            var zona = await zonaRepository.GetByIdAsync(jornada.ZonaUrbanaId);
            var totalInscritos = jornada.TotalInscritos;
            var ocupacionPct = jornada.VolunteerCapacity > 0
                ? (decimal)totalInscritos / jornada.VolunteerCapacity * 100
                : 0m;

            var especies = new List<JornadaEspecieDetailDto>();
            foreach (var detalle in jornada.DetalleArboles)
            {
                var especie = await especieRepository.GetByIdAsync(detalle.EspecieArboreaId);
                especies.Add(new JornadaEspecieDetailDto
                {
                    EspecieId = especie.Id,
                    Nombre = especie.Name,
                    NombreCientifico = especie.ScientificName,
                    Cantidad = detalle.Quantity,
                    AlturaMaxMetros = especie.MaxHeightMeters
                });
            }

            return new JornadaDetailDto
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
                Estado = jornada.State.ToString(),
                Especies = especies
            };
        }
    }
}
