using EcoBrotes.Domain.Common;
using JornadaEntity = EcoBrotes.Domain.JornadasReforestacion.Entity.JornadaReforestacion;
using EcoBrotes.Domain.JornadasReforestacion.Port;

namespace EcoBrotes.Domain.JornadasReforestacion.Service
{
    [DomainService]
    public class SaveJornadaService(IJornadaReforestacionRepository jornadaRepository)
    {
        public async Task<Guid> ExecuteAsync(JornadaEntity jornada)
        {
            var jornadaAdded = await jornadaRepository.AddAsync(jornada);
            return jornadaAdded.Id;
        }
    }
}
